using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Azure.Storage.Blobs;
using Gratify.Api.Components;
using Gratify.Api.Database;
using Iso20022.Pain;
using Iso20022.Pain.V3;
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
        private readonly BlobServiceClient _blobClient;

        public InitiateCreditTransfer(IServiceProvider services, TelemetryClient telemetry, SlackService slackService, DebitorInformation debitor, BlobServiceClient blobClient)
        {
            _services = services;
            _telemetry = telemetry;
            _slackService = slackService;
            _debitor = debitor;
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
                    await UploadDocument(containerClient, initiation);
                    await database.SaveChangesAsync();
                }

                await Task.Delay(10000, token);
            }
        }

        private async Task UploadDocument(BlobContainerClient client, TransferInitiation initiation)
        {
            var serializer = new XmlSerializer(typeof(Document));
            var settings = new XmlWriterSettings
            {
                Indent = true,
                NewLineChars = "\r\n"
            };

            using var stream = new MemoryStream();
            using var writer = XmlWriter.Create(stream, settings);

            serializer.Serialize(writer, initiation.Document);

            var fileName = initiation.FileName();
            var blobClient = client.GetBlobClient(fileName);
            stream.Position = 0;
            await blobClient.UploadAsync(stream);

            stream.Close();
        }
    }
}