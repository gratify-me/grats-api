using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gratify.Api.Components;
using Gratify.Api.Database;
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

        public NotifyGratsDenied(IServiceProvider services, TelemetryClient telemetry, SlackService slackService)
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

                var pendingDenials = await database.Denials
                    .Include(grats => grats.Review)
                        .ThenInclude(review => review.Grats)
                    .Where(denial => !denial.IsNotified)
                    .Take(10)
                    .ToArrayAsync();

                foreach (var denial in pendingDenials)
                {
                    var notifyAuthor = components.NotifyGratsSent.UpdateDenied(denial);
                    await _slackService.UpdateMessage(notifyAuthor);
                    denial.IsNotified = true;
                    await database.SaveChangesAsync();
                }

                await Task.Delay(5000, token);
            }
        }
    }
}