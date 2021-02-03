using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gratify.Api.Test.Security
{
    public class FakeAuthenticationHandler : AuthenticationHandler<FakeAuthenticationOptions>
    {
        public FakeAuthenticationHandler(IOptionsMonitor<FakeAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            await Task.CompletedTask;

            var claimsIdentity = new ClaimsIdentity(nameof(FakeAuthenticationHandler));
            var principal = new ClaimsPrincipal(claimsIdentity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
