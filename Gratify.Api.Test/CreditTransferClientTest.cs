using Gratify.Api.Services;
using Iso20022.Pain;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Gratify.Api.Test
{
    public class CreditTransferClientTest
    {
        [Fact(Skip = "Only used for manual development on CreditTransferClient")]
        public void ShouldUploadTransferInitiation()
        {
            var configuration = Generate.Configuration();
            var debitor = configuration.GetSection(nameof(DebitorInformation)).Get<DebitorInformation>();
            var creditor = configuration.GetSection(nameof(CreditorInformation)).Get<CreditorInformation>();
            var initiation = Generate.TransferInitiation(debitor, creditor);

            var settings = configuration.GetSection(nameof(CreditTransferSettings)).Get<CreditTransferSettings>();
            var client = new CreditTransferClient(settings, Generate.TelemetryClient());

            var success = client.UploadTransferInitiation(initiation);

            Assert.True(success);
        }
    }
}
