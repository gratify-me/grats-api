using System.Net.Http.Headers;
using Gratify.Grats.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Gratify.Grats.Api
{
    // https://api.slack.com/docs/verifying-requests-from-slack
    public class Startup
    {
        private readonly OpenApiInfo _apiInfo = new OpenApiInfo
        {
            Version = "v1",
            Title = "Grats API",
            Description = "An API for sending, approving and receiving Grats.",
        };

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // https://docs.microsoft.com/en-us/azure/azure-monitor/app/asp-net-core
            services.AddApplicationInsightsTelemetry();
            // https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
            services.AddHttpClient<ISlackService, SlackService>(client =>
            {
                if (Configuration["SlackApiToken"] != null)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Configuration["SlackApiToken"]);
                }
            });
            services.AddSwaggerGen(c => c.SwaggerDoc(_apiInfo.Version, _apiInfo));
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder application, IWebHostEnvironment environment)
        {
            // TODO: This is nice for prototyping, but should be removed in production, and put inside "if (environment.IsDevelopment()) {...}"
            application
                .UseDeveloperExceptionPage()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", _apiInfo.Title);
                });

            application
                .UseSwagger()
                .UseHttpsRedirection()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
