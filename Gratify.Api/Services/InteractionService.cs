using System;
using System.Linq;
using System.Threading.Tasks;
using Gratify.Api.Database;
using Gratify.Api.Database.Entities;
using Gratify.Api.Messages;
using Gratify.Api.Modals;
using Microsoft.EntityFrameworkCore;
using Slack.Client.Chat;

namespace Gratify.Api.Services
{
    public class InteractionService
    {
        private readonly SlackService _slackService;
        private readonly GratsDb _database;

        public InteractionService(SlackService slackService, GratsDb database)
        {
            _slackService = slackService;
            _database = database;
        }

        public async Task SaveDraft(Draft draft)
        {
            await _database.AddAsync(draft);
            await _database.SaveChangesAsync();
        }

        public async Task SubmitGratsForReview(Grats grats)
        {
            var draft = await _database.IncompleteDrafts.SingleOrDefaultAsync(draft => draft.CorrelationId == grats.CorrelationId);
            if (draft == null)
            {
                return; // TODO: This should be logged.
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
                return; // TODO: This should be logged.
            }

            var forwardGrats = new ForwardGrats(this);
            var modal = forwardGrats.Modal(review);
            await _slackService.OpenModal(triggerId, modal);
        }

        public async Task ForwardReview(Review newReview, bool transferReviewResponsibility) // TODO: Handle transfer of responsibility.
        {
            var review = await _database.IncompleteReviews.SingleOrDefaultAsync(review => review.CorrelationId == newReview.CorrelationId);
            if (review == null)
            {
                return; // TODO: This should be logged.
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
                return; // TODO: This should be logged.
            }

            await _slackService.ReplyToInteraction(responseUrl, respondWith);

            approval.Review = review;
            approval.TeamId = review.TeamId;
            await _database.AddAsync(approval);
            await _database.SaveChangesAsync();

            var gratsReceived = new GratsReceived(this);
            var blocks = gratsReceived.Message(approval);
            var channel = await _slackService.GetAppChannel(review.Grats.Recipient);
            blocks.Channel = channel.Id;
            await _slackService.SendMessage(blocks);
        }

        public async Task DenyGrats(Denial denial, ResponseMessage respondWith, string responseUrl)
        {
            var review = await _database.IncompleteReviews.SingleOrDefaultAsync(review => review.CorrelationId == denial.CorrelationId);
            if (review == null)
            {
                return; // TODO: This should be logged.
            }

            await _slackService.ReplyToInteraction(responseUrl, respondWith);

            denial.Review = review;
            denial.TeamId = review.TeamId;
            await _database.AddAsync(denial);
            await _database.SaveChangesAsync();
        }

        public async Task OpenAddTeamMember(string triggerId)
        {
            var addTeamMember = new AddTeamMember(this);
            var modal = addTeamMember.Modal();
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
                return; // TODO: This should be logged.
            }

            teamMember.DefaultReviewer = null;
            await _database.SaveChangesAsync();
            await ShowAppHome(userId);
        }

        public async Task ShowAppHome(string userId)
        {
            var appHome = new ShowAppHome(this, _database);
            var homeBlocks = await appHome.HomeTab(userId);

            var test = await _slackService.PublishModal(userId, homeBlocks);
        }

        private async Task RequestReview(Review review)
        {
            var requestGratsReview = new RequestGratsReview(this);
            var blocks = requestGratsReview.Message(review);
            var channel = await _slackService.GetAppChannel(review.Reviewer);
            blocks.Channel = channel.Id;
            var response = await _slackService.SendMessage(blocks);
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
    }
}