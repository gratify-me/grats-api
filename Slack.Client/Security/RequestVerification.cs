using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Slack.Client.Security
{
    /// <summary>
    /// Verifies incoming requests from Slack using the validation procedure described in:
    /// https://api.slack.com/authentication/verifying-requests-from-slack
    /// </summary>
    public class RequestVerification
    {
        private readonly string _signature;

        private readonly string _requestTimestamp;

        public RequestVerification(string signature, string requestTimestamp)
        {
            _signature = signature;
            _requestTimestamp = requestTimestamp;
        }

        public static RequestVerification From(Dictionary<string, string> headers)
        {
            if (headers.TryGetValue("X-Slack-Signature", out var signature) &&
                headers.TryGetValue("X-Slack-Request-Timestamp", out var timestamp))
            {
                return new RequestVerification(signature.ToString(), timestamp.ToString());
            }

            return null;
        }

        public bool VerifySignature(string requestBody, string signingSecret)
        {
            if (!IsTimestampRecent())
            {
                return false;
            }

            var signatureBaseString = string.Join(":", Version, _requestTimestamp, requestBody);
            var hash = ComputeHash(signingSecret, signatureBaseString).Replace("-", string.Empty);
            var requestSignature = string.Concat($"{Version}=", hash);

            return requestSignature.ToUpperInvariant() == _signature.ToUpperInvariant();
        }

        private string Version => _signature.Split('=').First();

        private string ComputeHash(string key, string buffer)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var bufferBytes = Encoding.UTF8.GetBytes(buffer);

            using var hmac = new HMACSHA256(keyBytes);
            var hashBytes = hmac.ComputeHash(bufferBytes);

            return BitConverter.ToString(hashBytes);
        }

        private bool IsTimestampRecent()
        {
            var test = Math.Abs(DateTimeOffset.UtcNow.ToUnixTimeSeconds() - long.Parse(_requestTimestamp));
            if (test > 60 * 5)
            {
                // The request timestamp differs by more than 5 minutes from the server time.
                // This could indicate a replay attack, so we should ignore it.
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
