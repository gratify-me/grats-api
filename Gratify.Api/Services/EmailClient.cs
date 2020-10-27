using System;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.ApplicationInsights;
using MimeKit;

namespace Gratify.Api.Services
{
    public class EmailClient
    {
        private readonly EmailSettings _settings;
        private readonly TelemetryClient _telemetry;

        public EmailClient(EmailSettings settings, TelemetryClient telemetry)
        {
            _settings = settings;
            _telemetry = telemetry;
        }

        public async Task<bool> SendMail(string subject, string body, CancellationToken token = default)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            message.To.Add(new MailboxAddress(_settings.ToName, _settings.ToEmail));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, false);
                await client.AuthenticateAsync(_settings.Username, _settings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return true;
            }
            catch (Exception error)
            {
                _telemetry.TrackException(error);

                return false;
            }
        }
    }
}