using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gratify.Api.Components;
using Gratify.Api.Database;
using Iso20022.Pain;
using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Slack.Client;

namespace Gratify.Api.Services
{
    public class InitiateCreditTransfer : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly TelemetryClient _telemetry;
        private readonly SlackService _slackService;
        private readonly DebitorInformation _debitor;

        public InitiateCreditTransfer(IServiceProvider services, TelemetryClient telemetry, SlackService slackService, DebitorInformation debitor)
        {
            _services = services;
            _telemetry = telemetry;
            _slackService = slackService;
            _debitor = debitor;
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                using var scope = _services.CreateScope();
                var database = scope.ServiceProvider.GetRequiredService<GratsDb>();
                var components = scope.ServiceProvider.GetRequiredService<ComponentsService>();

                var pendingReceivals = await database.Receivals
                    .Where(receival => !receival.CreditTransferInitiated)
                    .Take(10)
                    .ToArrayAsync();

                if (pendingReceivals.Any())
                {
                    foreach (var receival in pendingReceivals)
                    {
                        receival.CreditTransferInitiated = true;
                    }

                    var transactions = pendingReceivals
                        .Select(receival => new CreditTransaction(
                            correlationId: receival.CorrelationId,
                            creditorName: receival.ReceiverName,
                            creditorAccount: receival.ReceiverAccountNumber,
                            amountNok: receival.AmountReceived));

                    var initiation = new TransferInitiation(_debitor, transactions.ToArray());
                    var document = initiation.Document;

                    await database.SaveChangesAsync();
                }

                await Task.Delay(10000, token);
            }
        }
    }
}