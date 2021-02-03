using Microsoft.AspNetCore.Authentication;

namespace Gratify.Api.Test.Security
{
    public class FakeAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string AllowAnonymous = "AllowAnonymous";
    }
}
