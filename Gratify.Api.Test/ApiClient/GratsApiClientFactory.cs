using Gratify.Api.Test.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gratify.Api.Test.ApiClient
{
    public class GratsApiClientFactory<TStartup> : WebApplicationFactory<TStartup>
    where TStartup : class
    {
        public GratsApiClient CreateApiClient()
        {
            var httpClient = CreateClient();
            return new GratsApiClient(httpClient);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder
                .ConfigureAppConfiguration(config => config.AddJsonFile("appsettings.json"))
                .ConfigureServices(services =>
                {
                    services
                        .AddAuthentication(options =>
                        {
                            options.DefaultScheme = FakeAuthenticationOptions.AllowAnonymous;
                        })
                        .AddScheme<FakeAuthenticationOptions, FakeAuthenticationHandler>(FakeAuthenticationOptions.AllowAnonymous, opt => { });

                    services.AddAuthorization(options =>
                    {
                        options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                    });
                });
        }
    }
}