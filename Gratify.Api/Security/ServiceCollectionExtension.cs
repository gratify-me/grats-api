using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Gratify.Api.Security
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddSlackAuthentication(this IServiceCollection services, string slackApiSigningSecret)
        {
            if (string.IsNullOrEmpty(slackApiSigningSecret))
            {
                throw new ArgumentNullException(nameof(slackApiSigningSecret));
            }

            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = SlackAuthenticationOptions.VerifyMessageSignature;
                })
                .AddScheme<SlackAuthenticationOptions, SlackAuthenticationHandler>(SlackAuthenticationOptions.VerifyMessageSignature, options =>
                {
                    options.SigningSecret = slackApiSigningSecret;
                });

            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            });

            return services;
        }

        public static IApplicationBuilder UseSlackAuthentication(this IApplicationBuilder application) =>
            application
                .UseAuthentication()
                .UseAuthorization();
    }
}
