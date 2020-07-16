using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gratify.Api.Components;
using Gratify.Api.Database;
using Gratify.Api.Database.Entities;
using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Slack.Client;

namespace Gratify.Api.Services
{
    public class SubmitGratsForReview : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly TelemetryClient _telemetry;
        private readonly SlackService _slackService;

        public SubmitGratsForReview(IServiceProvider services, TelemetryClient telemetry, SlackService slackService)
        {
            _services = services;
            _telemetry = telemetry;
            _slackService = slackService;
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                using var scope = _services.CreateScope();
                var database = scope.ServiceProvider.GetRequiredService<GratsDb>();
                var components = scope.ServiceProvider.GetRequiredService<ComponentsService>();

                var pendingGrats = await database.Grats
                    .Where(grats => !grats.Reviews.Any())
                    .Take(10)
                    .ToArrayAsync();

                foreach (var grats in pendingGrats)
                {
                    await RequestReview(database, components, grats);
                }

                await Task.Delay(1000, token);
            }
        }

        private async Task RequestReview(GratsDb database, ComponentsService component, Grats grats)
        {
            var notifyAuthor = await component.NotifyGratsSent.Message(grats);
            var authorNotification = await _slackService.SendMessage(notifyAuthor);

            var reviewer = await FindReviewerOrDefault(database, grats.Recipient, grats.TeamId);
            if (reviewer == default)
            {
                reviewer = grats.Author;
            }

            var review = new Review(
                correlationId: grats.CorrelationId,
                requestedAt: DateTime.UtcNow,
                reviewer: reviewer,
                authorNotification: authorNotification);

            review.Grats = grats;
            review.TeamId = grats.TeamId;

            var reviewMessage = await component.ReviewGrats.Message(review);
            review.SetReviewRequest(await _slackService.SendMessage(reviewMessage));

            await database.AddAsync(review);
            await database.SaveChangesAsync();

            var notifyAuthorPending = component.NotifyGratsSent.UpdatePendingApproval(review);
            await _slackService.UpdateMessage(notifyAuthorPending);
        }

        private async Task<string> FindReviewerOrDefault(GratsDb database, string userId, string teamId)
        {
            var defaultReviewer = await database.Users
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
            return await database.Users
                .AsNoTracking()
                .Where(user => user.TeamId == teamId)
                .Where(user => user.HasReports)
                .Select(user => user.UserId)
                .SingleOrDefaultAsync();
        }
    }
}