using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Iso20022.Pain;
using Iso20022.Pain.V3;
using Xunit;

namespace Gratify.Api.Test
{
    public class Iso20022Test
    {
        [Fact]
        public void GenerateXmlTest()
        {
            var debitor = new DebitorInformation
            {
                Name = "INITECH AS",
                OrgNr = 123456789,
                AccountNumber = "11987654321",
                BankBusinessRegistryCode = "DEVTESTCODE",
                BankFileTransferId = 1010,
                CustomerFileTransferId = "SWEDANOR",
            };

            var transaction = new CreditTransaction(
                correlationId: Guid.NewGuid(),
                creditorName: "Astri Iversen",
                creditorAccount: "12345678911",
                amountNok: 5);

            var initiation = new TransferInitiation(debitor, new CreditTransaction[]
            {
                transaction,
            });

            // WriteToFile(initiation);
            Assert.NotNull(initiation.Document);
        }

        private void WriteToFile(TransferInitiation initiation)
        {
            var serializer = new XmlSerializer(typeof(Document));
            var settings = new XmlWriterSettings
            {
                Indent = true,
                NewLineChars = "\r\n"
            };

            var path = Path.Join(Environment.CurrentDirectory, "/output");
            Directory.CreateDirectory(path);

            var fileName = Path.Join(path, initiation.FileName());
            using (var writer = XmlWriter.Create(fileName, settings))
            {
                serializer.Serialize(writer, initiation.Document);
            }

            Console.WriteLine($"Created file: {fileName}");
        }
    }
}
