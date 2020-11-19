using System;
using System.IO;
using Iso20022.Pain;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;

namespace Gratify.Api.Test
{
    public static class Generate
    {
        public static TransferInitiation TransferInitiation(DebitorInformation debitor, CreditorInformation creditor)
        {
            var transaction = new CreditTransaction(
                correlationId: Guid.NewGuid(),
                creditorName: creditor.Name,
                creditorAccount: creditor.AccountNumber,
                amountNok: 1);

            return new TransferInitiation(debitor, new CreditTransaction[]
            {
                transaction,
            });
        }

        public static TelemetryClient TelemetryClient()
        {
            var configuration = new TelemetryConfiguration();
            configuration.TelemetryChannel = new FakeTelemetryChannel();
            configuration.InstrumentationKey = Guid.NewGuid().ToString();
            configuration.TelemetryInitializers.Add(new OperationCorrelationTelemetryInitializer());

            return new TelemetryClient(configuration);
        }

        public static IConfiguration Configuration() =>
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
    }
}
