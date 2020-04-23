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
using Slack.Client.Chat;
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
        private readonly AddTeamMember _addTeamMember;
        private readonly ShowAppHome _showAppHome;
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
            _addTeamMember = new AddTeamMember(this);
            _showAppHome = new ShowAppHome(this, _database);
            _requestGratsReview = new RequestGratsReview(this);
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

            var reviewer = await FindReviewerOrDefault(grats.Recipient, draft.TeamId);
            if (reviewer == default)
            {
                reviewer = draft.Author;
            }

            var review = new Review(
                correlationId: grats.CorrelationId,
                requestedAt: DateTime.UtcNow,
                reviewer: reviewer);

            grats.Draft = draft;
            grats.TeamId = draft.TeamId;
            await _database.AddAsync(grats);

            review.Grats = grats;
            review.TeamId = draft.TeamId;
            await _database.AddAsync(review);
            await _database.SaveChangesAsync();

            await RequestReview(review);
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
            var review = await _database.IncompleteReviews.SingleOrDefaultAsync(review => review.CorrelationId == correlationId);
            if (review == default)
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

            var newReview = review.ForwardTo(newReviewerId);
            await _database.AddAsync(newReview);
            await _database.SaveChangesAsync();

            await RequestReview(newReview);
        }

        public async Task ApproveGrats(Approval approval, ResponseMessage respondWith, string responseUrl)
        {
            var review = await _database.IncompleteReviews.SingleOrDefaultAsync(review => review.CorrelationId == approval.CorrelationId);
            if (review == default)
            {
                TrackEntity($"{nameof(ApproveGrats)}: Approval not found", approval);
                return;
            }

            await _slackService.ReplyToInteraction(responseUrl, respondWith);

            approval.Review = review;
            approval.TeamId = review.TeamId;
            await _database.AddAsync(approval);
            await _database.SaveChangesAsync();

            var blocks = _gratsReceived.Message(approval);
            var channel = await _slackService.GetAppChannel(review.Grats.Recipient);
            blocks.Channel = channel.Id;
            await _slackService.SendMessage(blocks);
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
            await ShowAppHome(userId);
        }

        public async Task RemoveTeamMember(string userId, int teamMemberId)
        {
            var teamMember = await _database.Users.FindAsync(teamMemberId);
            if (teamMember == null)
            {
                TrackUserId($"{nameof(RemoveTeamMember)}: User not found", userId);
                return;
            }

            teamMember.DefaultReviewer = null;
            await _database.SaveChangesAsync();
            await ShowAppHome(userId);
        }

        public async Task ShowAppHome(string userId)
        {
            var homeBlocks = await _showAppHome.HomeTab(userId);
            await _slackService.PublishModal(userId, homeBlocks);
        }

        private async Task RequestReview(Review review)
        {
            var requestGratsReview = _requestGratsReview;
            var blocks = requestGratsReview.Message(review);
            var channel = await _slackService.GetAppChannel(review.Reviewer);
            blocks.Channel = channel.Id;
            await _slackService.SendMessage(blocks);
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

        private async Task<Settings> FindSettings(string teamId, string userId)
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

            return defaultSettings;
        }
    }
}