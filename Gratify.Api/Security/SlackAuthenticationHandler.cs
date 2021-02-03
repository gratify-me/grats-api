using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Slack.Client.Security;

namespace Gratify.Api.Security
{
    public class SlackAuthenticationHandler : AuthenticationHandler<SlackAuthenticationOptions>
    {
        public SlackAuthenticationHandler(IOptionsMonitor<SlackAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var allowAnonymousAttribute = Context.GetEndpoint()?.Metadata?.GetMetadata<IAllowAnonymous>();
            if (allowAnonymousAttribute != null)
            {
                Logger.LogInformation("AllowAnonymous attribute found.");
                return AuthenticateResult.NoResult();
            }

            var headers = Request.Headers.ToDictionary(pair => pair.Key, pair => pair.Value.ToString());
            var verification = RequestVerification.From(headers);
            if (verification == null)
            {
                Logger.LogError("Missing verification headers.");
                return AuthenticateResult.Fail("Missing verification headers.");
            }

            var requestBody = await ReadRequestBody();
            var isVerified = verification.VerifySignature(requestBody, Options.SigningSecret);
            if (isVerified)
            {
                var claimsIdentity = new ClaimsIdentity(nameof(SlackAuthenticationHandler));
                var principal = new ClaimsPrincipal(claimsIdentity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }
            else
            {
                Logger.LogError("Request verification failed.");
                return AuthenticateResult.Fail("Request verification failed.");
            }
        }

        /*
         * Reading the request body in any middleware requires some adjustments,
         * since by default, it can only be read once. This is a problem,
         * when we need to read the body twice: first in the authentication handler,
         * and then when passing the request body to the controller action.
         *
         * In the following function, we enable buffering, in order to facilitate
         * multiple reads of the request body.
         *
         * Learn more at: https://devblogs.microsoft.com/aspnet/re-reading-asp-net-core-request-bodies-with-enablebuffering/
         */
        private async Task<string> ReadRequestBody()
        {
            Request.EnableBuffering();

            using var reader = new StreamReader(
                Request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 1024,
                leaveOpen: true);

            var requestBody = await reader.ReadToEndAsync();
            Request.Body.Position = 0;

            return requestBody;
        }
    }
}
