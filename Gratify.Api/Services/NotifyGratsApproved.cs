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
    public class NotifyGratsApproved : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly TelemetryClient _telemetry;
        private readonly SlackService _slackService;

        public NotifyGratsApproved(IServiceProvider services, TelemetryClient telemetry, SlackService slackService)
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

                var pendingApprovals = await database.Approvals
                    .Include(grats => grats.Review)
                        .ThenInclude(review => review.Grats)
                    .Where(approval => approval.ReceiverNotificationTimestamp == null)
                    .Take(10)
                    .ToArrayAsync();

                foreach (var approval in pendingApprovals)
                {
                    var blocks = await components.GratsReceived.Message(approval);
                    var channel = await _slackService.GetAppChannel(approval.Review.Grats.Recipient);
                    blocks.Channel = channel.Id;
                    var receiverNotification = await _slackService.SendMessage(blocks);
                    approval.SetReceiverNotification(receiverNotification);

                    var notifyAuthor = components.NotifyGratsSent.UpdateApproved(approval);
                    await _slackService.UpdateMessage(notifyAuthor);

                    await database.SaveChangesAsync();
                }

                await Task.Delay(5000, token);
            }
        }
    }
}