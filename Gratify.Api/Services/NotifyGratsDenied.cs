using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gratify.Api.Database;
using Gratify.Api.Database.Entities;
using Gratify.Api.Messages;
using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Slack.Client;

namespace Gratify.Api.Services
{
    public class NotifyGratsDenied : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly TelemetryClient _telemetry;
        private readonly SlackService _slackService;
        private readonly NotifyGratsSent _notifyGratsSent;
        private readonly RequestGratsReview _requestGratsReview;

        public NotifyGratsDenied(
            IServiceProvider services,
            TelemetryClient telemetry,
            SlackService slackService)
        {
            _services = services;
            _telemetry = telemetry;
            _slackService = slackService;
            _notifyGratsSent = new NotifyGratsSent(_slackService);
            _requestGratsReview = new RequestGratsReview(_slackService);
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                using var scope = _services.CreateScope();
                var database = scope.ServiceProvider.GetRequiredService<GratsDb>();
                var pendingDenials = await database.Denials
                    .Include(grats => grats.Review)
                        .ThenInclude(review => review.Grats)
                            .ThenInclude(grats => grats.Draft)
                    .Where(denial => !denial.IsNotified)
                    .Take(10)
                    .ToArrayAsync();

                foreach (var denial in pendingDenials)
                {
                    await NotifyDenied(denial);
                    denial.IsNotified = true;
                    await database.SaveChangesAsync();
                }

                await Task.Delay(1000, token);
            }
        }

        private async Task NotifyDenied(Denial denial)
        {
            var notifyAuthor = _notifyGratsSent.UpdateDenied(denial);
            await _slackService.UpdateMessage(notifyAuthor);
        }
    }
}