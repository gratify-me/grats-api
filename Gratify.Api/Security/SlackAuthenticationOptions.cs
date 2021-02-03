using Microsoft.AspNetCore.Authentication;

namespace Gratify.Api.Security
{
    public class SlackAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string VerifyMessageSignature = "VerifyMessageSignature";

        public string SigningSecret { get; set; }
    }
}
