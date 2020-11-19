using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
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
        private const string ContainerNamePrefix = "iso20022-pain-001-001-03-documents";
        private readonly IServiceProvider _services;
        private readonly TelemetryClient _telemetry;
        private readonly SlackService _slackService;
        private readonly DebitorInformation _debitor;
        private readonly CreditTransferClient _transferClient;
        private readonly BlobServiceClient _blobClient;

        public InitiateCreditTransfer(IServiceProvider services, TelemetryClient telemetry, SlackService slackService, DebitorInformation debitor, CreditTransferClient transferClient, BlobServiceClient blobClient)
        {
            _services = services;
            _telemetry = telemetry;
            _slackService = slackService;
            _debitor = debitor;
            _transferClient = transferClient;
            _blobClient = blobClient;
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            var containerName = $"{ContainerNamePrefix}-{DateTime.Now.ToFileTimeUtc()}";
            var containerClient = await _blobClient.CreateBlobContainerAsync(containerName);

            while (!token.IsCancellationRequested)
            {
                using var scope = _services.CreateScope();
                var database = scope.ServiceProvider.GetRequiredService<GratsDb>();
                var components = scope.ServiceProvider.GetRequiredService<ComponentsService>();

                var pendingReceivals = await database.Receivals
                    .Include(receival => receival.Approval)
                        .ThenInclude(approval => approval.Review)
                            .ThenInclude(review => review.Grats)
                    .Where(receival => !receival.CreditTransferInitiated)
                    .Take(10)
                    .ToArrayAsync();

                if (pendingReceivals.Any())
                {
                    foreach (var receival in pendingReceivals)
                    {
                        receival.CreditTransferInitiated = true;

                        var notifyReceiver = await components.GratsReceived.UpdateMoneySent(receival);
                        await _slackService.UpdateMessage(notifyReceiver);
                    }

                    var transactions = pendingReceivals
                        .Select(receival => new CreditTransaction(
                            correlationId: receival.CorrelationId,
                            creditorName: receival.ReceiverName,
                            creditorAccount: receival.ReceiverAccountNumber,
                            amountNok: receival.AmountReceived));

                    var initiation = new TransferInitiation(_debitor, transactions.ToArray());
                    _transferClient.UploadTransferInitiation(initiation);
                    await UploadDocument(containerClient, initiation);
                    await database.SaveChangesAsync();
                }

                await Task.Delay(10000, token);
            }
        }

        private async Task UploadDocument(BlobContainerClient client, TransferInitiation initiation)
        {
            using var stream = new MemoryStream();
            initiation.WriteToStream(stream);

            var fileName = initiation.FileName();
            var blobClient = client.GetBlobClient(fileName);
            await blobClient.UploadAsync(stream);

            stream.Close();
        }
    }
}