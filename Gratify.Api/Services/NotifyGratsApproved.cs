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
    public class NotifyGratsApproved : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly TelemetryClient _telemetry;
        private readonly SlackService _slackService;
        private readonly NotifyGratsSent _notifyGratsSent;
        private readonly RequestGratsReview _requestGratsReview;
        private readonly GratsReceived _gratsReceived;

        public NotifyGratsApproved(
            IServiceProvider services,
            TelemetryClient telemetry,
            SlackService slackService)
        {
            _services = services;
            _telemetry = telemetry;
            _slackService = slackService;
            _notifyGratsSent = new NotifyGratsSent(_slackService);
            _requestGratsReview = new RequestGratsReview(_slackService);
            _gratsReceived = new GratsReceived();
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                using var scope = _services.CreateScope();
                var database = scope.ServiceProvider.GetRequiredService<GratsDb>();
                var pendingApprovals = await database.Approvals
                    .Include(grats => grats.Review)
                        .ThenInclude(review => review.Grats)
                            .ThenInclude(grats => grats.Draft)
                    .Where(approval => !approval.IsNotified)
                    .Take(10)
                    .ToArrayAsync();

                foreach (var approval in pendingApprovals)
                {
                    await NotifyApproved(approval);
                    approval.IsNotified = true;
                    await database.SaveChangesAsync();
                }

                await Task.Delay(1000, token);
            }
        }

        private async Task NotifyApproved(Approval approval)
        {
            var blocks = _gratsReceived.Message(approval);
            var channel = await _slackService.GetAppChannel(approval.Review.Grats.Recipient);
            blocks.Channel = channel.Id;
            await _slackService.SendMessage(blocks);

            var notifyAuthor = _notifyGratsSent.UpdateApproved(approval);
            await _slackService.UpdateMessage(notifyAuthor);
        }
    }
}