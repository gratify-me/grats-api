using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Iso20022.Pain;
using Iso20022.Pain.V3;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Gratify.Api.Test
{
    public class Iso20022Test
    {
        [Fact]
        public void ShouldGenerateDocument()
        {
            var configuration = Generate.Configuration();
            var debitor = configuration.GetSection(nameof(DebitorInformation)).Get<DebitorInformation>();
            var creditor = configuration.GetSection(nameof(CreditorInformation)).Get<CreditorInformation>();
            var initiation = Generate.TransferInitiation(debitor, creditor);

            Assert.NotNull(initiation.Document);
        }

        [Fact(Skip = "Only used for manual generation of Iso20022.Pain.V3 document")]
        public void ShouldGenerateXmlFile()
        {
            var configuration = Generate.Configuration();
            var debitor = configuration.GetSection(nameof(DebitorInformation)).Get<DebitorInformation>();
            var creditor = configuration.GetSection(nameof(CreditorInformation)).Get<CreditorInformation>();
            var initiation = Generate.TransferInitiation(debitor, creditor);

            WriteToFile(initiation);

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
