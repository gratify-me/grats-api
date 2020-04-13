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

namespace Gratify.Api.Services
{
    public class InteractionService
    {
        private readonly TelemetryClient _telemetry;
        private readonly SlackService _slackService;
        private readonly GratsDb _database;
        private readonly SendGrats _sendGrats;
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

            var modal = _sendGrats.Modal(draft, userId);
            await _slackService.OpenModal(triggerId, modal);
        }

        public async Task SubmitGratsForReview(Grats grats)
        {
            var draft = await _database.IncompleteDrafts.SingleOrDefaultAsync(draft => draft.CorrelationId == grats.CorrelationId);
            if (draft == null)
            {
                TrackEntity($"{nameof(SubmitGratsForReview)}: Draft not found", grats);
                return;
            }

            var reviewer = await GetReviewerFor(draft.Author, draft.TeamId);
            if (reviewer == null)
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

        public async Task OpenForwardReview(Guid correlationId, string triggerId)
        {
            var review = await _database.IncompleteReviews.SingleOrDefaultAsync(review => review.CorrelationId == correlationId);
            if (review == null)
            {
                TrackCorrelationId($"{nameof(OpenForwardReview)}: Review not found", correlationId);
                return;
            }

            var modal = _forwardGrats.Modal(review);
            await _slackService.OpenModal(triggerId, modal);
        }

        // TODO: Handle transfer of responsibility.
        public async Task ForwardReview(Review newReview, bool transferReviewResponsibility)
        {
            var review = await _database.IncompleteReviews.SingleOrDefaultAsync(review => review.CorrelationId == newReview.CorrelationId);
            if (review == null)
            {
                TrackEntity($"{nameof(SubmitGratsForReview)}: Review not found", newReview);
                return;
            }

            newReview.Grats = review.Grats;
            newReview.TeamId = review.TeamId;
            newReview.ForwardedFrom = review.Id;
            await _database.AddAsync(newReview);
            await _database.SaveChangesAsync();

            await RequestReview(newReview);
        }

        public async Task ApproveGrats(Approval approval, ResponseMessage respondWith, string responseUrl)
        {
            var review = await _database.IncompleteReviews.SingleOrDefaultAsync(review => review.CorrelationId == approval.CorrelationId);
            if (review == null)
            {
                TrackEntity($"{nameof(SubmitGratsForReview)}: Approval not found", approval);
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

        public async Task DenyGrats(Denial denial, ResponseMessage respondWith, string responseUrl)
        {
            var review = await _database.IncompleteReviews.SingleOrDefaultAsync(review => review.CorrelationId == denial.CorrelationId);
            if (review == null)
            {
                TrackEntity($"{nameof(SubmitGratsForReview)}: Denial not found", denial);
                return;
            }

            await _slackService.ReplyToInteraction(responseUrl, respondWith);

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
            if (teamMember == null)
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
                TrackUserId($"{nameof(SubmitGratsForReview)}: User not found", userId);
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

        private async Task<string> GetReviewerFor(string userId, string teamId)
        {
            var defaultReviewer = await _database.Users
                .AsNoTracking()
                .Where(user => user.TeamId == teamId)
                .Where(user => user.UserId == userId)
                .Select(user => user.DefaultReviewer)
                .SingleOrDefaultAsync();

            if (defaultReviewer != null)
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