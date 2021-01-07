using System;
using System.IO;
using Iso20022.Pain;
using Microsoft.ApplicationInsights;
using Renci.SshNet;

namespace Gratify.Api.Services
{
    public class CreditTransferClient
    {
        private readonly CreditTransferSettings _settings;
        private readonly TelemetryClient _telemetry;

        public CreditTransferClient(CreditTransferSettings settings, TelemetryClient telemetry)
        {
            _settings = settings;
            _telemetry = telemetry;
        }

        public bool UploadTransferInitiation(TransferInitiation initiation)
        {
            var success = false;
            using var client = new SftpClient(_settings.GetConnectionInfo());
            using var stream = new MemoryStream();
            try
            {
                initiation.WriteToStream(stream);

                client.Connect();

                var tempFilePath = $"{_settings.ToEvryDirectory}/{initiation.TempFileName(_settings.UseTestFiles)}";
                client.UploadFile(stream, tempFilePath);

                var trueFilePath = $"{_settings.ToEvryDirectory}/{initiation.FileName(_settings.UseTestFiles)}";
                client.RenameFile(tempFilePath, trueFilePath);

                success = true;
            }
            catch (Exception error)
            {
                _telemetry.TrackException(error);
            }
            finally
            {
                client.Disconnect();
            }

            return success;
        }
    }
}