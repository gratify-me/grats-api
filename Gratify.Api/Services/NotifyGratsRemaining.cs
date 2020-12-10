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
    public class NotifyGratsRemaining : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly TelemetryClient _telemetry;
        private readonly SlackService _slackService;

        public NotifyGratsRemaining(IServiceProvider services, TelemetryClient telemetry, SlackService slackService)
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

                var users = await database.Users
                    .Where(user => user.IsEligibleForGrats)
                    .Where(user => !user.IsAdministrator)
                    .Where(user => !user.HasReports)
                    .Where(user => DateTime.UtcNow.Subtract(user.LastRemindedAt) > TimeSpan.FromDays(10))
                    .Take(100)
                    .ToArrayAsync(token);

                foreach (var user in users)
                {
                    var settings = await database.SettingsFor(user.TeamId, user.UserId);
                    var pendingAndApprovedGratsLastPeriod = database.PendingAndApprovedGratsFor(user.UserId, settings.GratsPeriodInDays);
                    if (await pendingAndApprovedGratsLastPeriod.CountAsync() < settings.NumberOfGratsPerPeriod)
                    {
                        try
                        {
                            var gratsReminder = await components.GratsRemaining.Message(user);
                            await _slackService.SendMessage(gratsReminder);

                            user.LastRemindedAt = DateTime.UtcNow;
                            await database.SaveChangesAsync();
                        }
                        catch (Exception error)
                        {
                            _telemetry.TrackException(error);
                        }
                    }
                }

                await Task.Delay(5000, token);
            }
        }
    }
}