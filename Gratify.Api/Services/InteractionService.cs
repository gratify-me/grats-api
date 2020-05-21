using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gratify.Api.Database;
using Gratify.Api.Database.Entities;
using Gratify.Api.Messages;
using Gratify.Api.Modals;
using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore;
using Slack.Client.Views;

namespace Gratify.Api.Services
{
    public class InteractionService
    {
        private readonly TelemetryClient _telemetry;
        private readonly SlackService _slackService;
        private readonly GratsDb _database;
        private readonly SendGrats _sendGrats;
        private readonly AllGratsSpent _allGratsSpent;
        private readonly DenyGrats _denyGrats;
        private readonly ForwardGrats _forwardGrats;
        private readonly GratsReceived _gratsReceived;
        private readonly ChangeSettings _changeSettings;
        private readonly AddTeamMember _addTeamMember;
        private readonly ShowAppHome _showAppHome;
        private readonly NotifyGratsSent _notifyGratsSent;
        private readonly RequestGratsReview _requestGratsReview;

        public InteractionService(
            TelemetryClient telemetry,
            SlackService slackService,
            GratsDb database)
        {
            _telemetry = telemetry;
            _slackService = slackService;
            _database = database;

            // TODO: Find a way to organize this that doesn't require a circular dependency.
            _sendGrats = new SendGrats(this);
            _allGratsSpent = new AllGratsSpent(this);
            _denyGrats = new DenyGrats(this);
            _forwardGrats = new ForwardGrats(this);
            _gratsReceived = new GratsReceived(this);
            _changeSettings = new ChangeSettings(this);
            _addTeamMember = new AddTeamMember(this);
            _showAppHome = new ShowAppHome(this, _database);
            _notifyGratsSent = new NotifyGratsSent(_slackService);
            _requestGratsReview = new RequestGratsReview(this, _slackService);
        }

        public async Task SendGrats(Draft draft, string triggerId, string userId = null)
        {
            await _database.AddAsync(draft);
            await _database.SaveChangesAsync();

            var settings = await FindSettings(draft.TeamId, draft.Author);
            // TODO: Include pending grats as well to avoid over-sending.
            var approvedGratsLastPeriod = _database.Approvals
                .Select(approval => approval.Review.Grats)
                .Where(grats => grats.Draft.Author == draft.Author)
                .Where(grats => grats.CreatedAt > DateTime.UtcNow.AddDays(-settings.GratsPeriodInDays))
                .OrderByDescending(grats => grats.CreatedAt);

            if (await approvedGratsLastPeriod.CountAsync() >= settings.NumberOfGratsPerPeriod)
            {
                var lastApprovedGrats = await approvedGratsLastPeriod.FirstAsync();
                var allGratsSpentModal = _allGratsSpent.Modal(draft.CorrelationId, lastApprovedGrats.CreatedAt, settings.GratsPeriodInDays);
                await _slackService.OpenModal(triggerId, allGratsSpentModal);
            }
            else
            {
                var modal = _sendGrats.Modal(draft, userId);
                await _slackService.OpenModal(triggerId, modal);
            }
        }

        public async Task<Modal> SendGratsAnyway(Guid correlationId)
        {
            var draft = await _database.IncompleteDrafts.SingleOrDefaultAsync(draft => draft.CorrelationId == correlationId);
            if (draft == null)
            {
                TrackCorrelationId($"{nameof(SendGratsAnyway)}: Draft not found", correlationId);
            }

            return _sendGrats.Modal(draft);
        }

        public async Task SubmitGratsForReview(Grats grats)
        {
            var draft = await _database.IncompleteDrafts.SingleOrDefaultAsync(draft => draft.CorrelationId == grats.CorrelationId);
            if (draft == default)
            {
                TrackEntity($"{nameof(SubmitGratsForReview)}: Draft not found", grats);
                return;
            }

            grats.Draft = draft;
            grats.TeamId = draft.TeamId;
            await _database.AddAsync(grats);

            var notifyAuthor = await _notifyGratsSent.Message(grats);
            var authorNotification = await _slackService.SendMessage(notifyAuthor);
            var reviewer = await FindReviewerOrDefault(grats.Recipient, draft.TeamId);
            if (reviewer == default)
            {
                reviewer = draft.Author;
            }

            var review = new Review(
                correlationId: grats.CorrelationId,
                requestedAt: DateTime.UtcNow,
                reviewer: reviewer,
                authorNotification: authorNotification);

            review.Grats = grats;
            review.TeamId = draft.TeamId;

            var reviewMessage = await _requestGratsReview.Message(review);
            review.SetReviewRequest(await _slackService.SendMessage(reviewMessage));

            await _database.AddAsync(review);
            await _database.SaveChangesAsync();

            var notifyAuthorPending = _notifyGratsSent.UpdatePendingApproval(review);
            await _slackService.UpdateMessage(notifyAuthorPending);
        }

        public async Task OpenDenyGrats(Guid correlationId, string triggerId)
        {
            var review = await _database.IncompleteReviews.SingleOrDefaultAsync(review => review.CorrelationId == correlationId);
            if (review == default)
            {
                TrackCorrelationId($"{nameof(OpenDenyGrats)}: Review not found", correlationId);
                return;
            }

            var modal = _denyGrats.Modal(review);
            await _slackService.OpenModal(triggerId, modal);
        }

        public async Task OpenForwardReview(Guid correlationId, string triggerId)
        {
            var review = await _database.IncompleteReviews.SingleOrDefaultAsync(review => review.CorrelationId == correlationId);
            if (review == default)
            {
                TrackCorrelationId($"{nameof(OpenForwardReview)}: Review not found", correlationId);
                return;
            }

            var modal = _forwardGrats.Modal(review);
            await _slackService.OpenModal(triggerId, modal);
        }

        public async Task ForwardReview(Guid correlationId, string newReviewerId, bool? transferReviewResponsibility)
        {
            var oldReview = await _database.IncompleteReviews.SingleOrDefaultAsync(review => review.CorrelationId == correlationId);
            if (oldReview == default)
            {
                TrackCorrelationId($"{nameof(ForwardReview)}: Review not found", correlationId);
                return;
            }

            if (transferReviewResponsibility.GetValueOrDefault(false))
            {
                // TODO: This might be combined into one query.
                var grats = await _database.Grats.SingleAsync(grats => grats.CorrelationId == correlationId);
                var user = await _database.Users.SingleAsync(user => user.UserId == grats.Recipient);
                user.DefaultReviewer = newReviewerId;
            }

            var newReview = oldReview.ForwardTo(newReviewerId);
            var reviewMessage = await _requestGratsReview.Message(newReview);
            newReview.SetReviewRequest(await _slackService.SendMessage(reviewMessage));

            await _database.AddAsync(newReview);
            await _database.SaveChangesAsync();

            var notifyOldReviewer = _requestGratsReview.UpdateForwarded(oldReview, newReview);
            await _slackService.UpdateMessage(notifyOldReviewer);

            var notifyAuthor = _notifyGratsSent.UpdateForwarded(newReview);
            await _slackService.UpdateMessage(notifyAuthor);
        }

        public async Task ApproveGrats(Approval approval)
        {
            var review = await _database.IncompleteReviews.SingleOrDefaultAsync(review => review.CorrelationId == approval.CorrelationId);
            if (review == default)
            {
                TrackEntity($"{nameof(ApproveGrats)}: Approval not found", approval);
                return;
            }

            approval.Review = review;
            approval.TeamId = review.TeamId;
            await _database.AddAsync(approval);
            await _database.SaveChangesAsync();

            var blocks = _gratsReceived.Message(approval);
            var channel = await _slackService.GetAppChannel(review.Grats.Recipient);
            blocks.Channel = channel.Id;
            await _slackService.SendMessage(blocks);

            var notifyReviewer = _requestGratsReview.UpdateApproved(approval);
            await _slackService.UpdateMessage(notifyReviewer);

            var notifyAuthor = _notifyGratsSent.UpdateApproved(approval);
            await _slackService.UpdateMessage(notifyAuthor);
        }

        public async Task DenyGrats(Denial denial)
        {
            var review = await _database.IncompleteReviews.SingleOrDefaultAsync(review => review.CorrelationId == denial.CorrelationId);
            if (review == default)
            {
                TrackEntity($"{nameof(DenyGrats)}: Denial not found", denial);
                return;
            }

            denial.Review = review;
            denial.TeamId = review.TeamId;
            await _database.AddAsync(denial);
            await _database.SaveChangesAsync();

            var notifyReviewer = _requestGratsReview.UpdateDenied(denial);
            await _slackService.UpdateMessage(notifyReviewer);

            var notifyAuthor = _notifyGratsSent.UpdateDenied(denial);
            await _slackService.UpdateMessage(notifyAuthor);
        }

        public async Task ChangeAccountDetails(Receival receival)
        {
            var approval = await _database.Approvals
                .Include(approval => approval.Review)
                    .ThenInclude(review => review.Grats)
                        .ThenInclude(grats => grats.Draft)
                .SingleOrDefaultAsync(approval => approval.CorrelationId == receival.CorrelationId);

            if (approval == default)
            {
                TrackEntity($"{nameof(ChangeAccountDetails)}: Approval not found", approval);
                return;
            }

            receival.Approval = approval;
            receival.TeamId = approval.TeamId;

            var notifyReviewer = _requestGratsReview.UpdateReceived(receival);
            await _slackService.UpdateMessage(notifyReviewer);

            var notifyAuthor = _notifyGratsSent.UpdateReceived(receival);
            await _slackService.UpdateMessage(notifyAuthor);
        }

        public async Task TransferToAccount(Receival receival)
        {
            await ChangeAccountDetails(receival);
        }

        public async Task OpenChangeSettings(string triggerId, string teamId, string userId)
        {
            var settings = await FindSettings(teamId, userId);
            var modal = _changeSettings.Modal(settings);
            await _slackService.OpenModal(triggerId, modal);
        }

        public async Task ChangeSettings(string teamId, string userId, int gratsPeriodInDays, int numberOfGratsPerPeriod)
        {
            var settings = await _database.Settings.SingleOrDefaultAsync(setting => setting.TeamId == teamId);
            if (settings == default)
            {
                TrackUserId($"{nameof(ChangeSettings)}: Settings not found", userId);
                return;
            }

            settings.GratsPeriodInDays = gratsPeriodInDays;
            settings.NumberOfGratsPerPeriod = numberOfGratsPerPeriod;
            settings.UpdatedAt = DateTime.UtcNow;
            settings.UpdatedBy = userId;

            await _database.SaveChangesAsync();
            await ShowAppHome(teamId, userId);
        }

        public async Task OpenAddTeamMember(string triggerId)
        {
            var modal = _addTeamMember.Modal();
            await _slackService.OpenModal(triggerId, modal);
        }

        public async Task AddTeamMember(string teamId, string userId, string teamMemberId)
        {
            var teamMember = await _database.Users.SingleOrDefaultAsync(user => user.UserId == teamMemberId);
            if (teamMember == default)
            {
                teamMember = new User(teamId, teamMemberId, defaultReviewer: userId);
                await _database.Users.AddAsync(teamMember);
            }
            else
            {
                teamMember.DefaultReviewer = userId;
            }

            await _database.SaveChangesAsync();
            await ShowAppHome(teamId, userId);
        }

        public async Task RemoveTeamMember(string teamId, string userId, int teamMemberId)
        {
            var teamMember = await _database.Users.FindAsync(teamMemberId);
            if (teamMember == null)
            {
                TrackUserId($"{nameof(RemoveTeamMember)}: User not found", userId);
                return;
            }

            teamMember.DefaultReviewer = null;
            await _database.SaveChangesAsync();
            await ShowAppHome(teamId, userId);
        }

        public async Task ShowAppHome(string teamId, string userId)
        {
            var homeBlocks = await _showAppHome.HomeTab(teamId, userId);
            await _slackService.PublishModal(userId, homeBlocks);
        }

        public async Task<Settings> FindSettings(string teamId, string userId)
        {
            // TODO: Settings should probably be created on installation. In addition we might want to cache settings.
            var maybeSettings = await _database.Settings.SingleOrDefaultAsync(settings => settings.TeamId == teamId);
            if (maybeSettings != default)
            {
                return maybeSettings;
            }

            var defaultSettings = new Settings(
                teamId: teamId,
                createdAt: DateTime.UtcNow,
                createdBy: userId);

            await _database.AddAsync(defaultSettings);
            await _database.SaveChangesAsync();

            return defaultSettings;
        }

        private async Task<string> FindReviewerOrDefault(string userId, string teamId)
        {
            var defaultReviewer = await _database.Users
                .AsNoTracking()
                .Where(user => user.TeamId == teamId)
                .Where(user => user.UserId == userId)
                .Select(user => user.DefaultReviewer)
                .SingleOrDefaultAsync();

            if (defaultReviewer != default)
            {
                return defaultReviewer;
            }

            // TODO: Round-robbin backup approver.
            return await _database.Users
                .AsNoTracking()
                .Where(user => user.TeamId == teamId)
                .Where(user => user.HasReports)
                .Select(user => user.UserId)
                .SingleOrDefaultAsync();
        }

        private void TrackEntity(string eventName, Entity entity) => TrackCorrelationId(eventName, entity.CorrelationId);

        private void TrackCorrelationId(string eventName, Guid correlationId)
        {
            _telemetry.TrackEvent(eventName, new Dictionary<string, string>
            {
                { "CorrelationId", correlationId.ToString() }
            });
        }

        private void TrackUserId(string eventName, string userId)
        {
            _telemetry.TrackEvent(eventName, new Dictionary<string, string>
            {
                { "UserId", userId }
            });
        }
    }
}