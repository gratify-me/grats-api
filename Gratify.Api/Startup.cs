using System.Net.Http.Headers;
using Azure.Storage.Blobs;
using Gratify.Api.Components;
using Gratify.Api.Database;
using Gratify.Api.Services;
using Iso20022.Pain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Slack.Client;
using Slack.Client.Chat.Converters;
using Slack.Client.Events.Converters;
using Slack.Client.Views.Converters;

namespace Gratify.Api
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
            services.AddHttpClient<SlackService>(client =>
            {
                if (Configuration["SlackApiBotUserToken"] != null)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Configuration["SlackApiBotUserToken"]);
                }
            });
            services.AddSwaggerGen(c => c.SwaggerDoc(_apiInfo.Version, _apiInfo));
            services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                    options.JsonSerializerOptions.Converters.Add(new EventWrapperConverter());
                    options.JsonSerializerOptions.Converters.Add(new ViewPayloadConverter());
                    options.JsonSerializerOptions.Converters.Add(new MessagePayloadConverter());
                });

            var databaseSettings = Configuration.GetSection("DatabaseSettings").Get<DatabaseSettings>();
            services.AddGratsDb(databaseSettings);

            services.AddScoped<ComponentsService>();

            var debitorInformation = Configuration.GetSection("DebitorInformation").Get<DebitorInformation>();
            var storageAccountConnectionString = Configuration.GetValue<string>("StorageAccountConnectionString");
            if (!string.IsNullOrEmpty(storageAccountConnectionString))
            {
                services.AddSingleton(debitorInformation);
                services.AddTransient(services => new BlobServiceClient(storageAccountConnectionString));
                services.AddHostedService<InitiateCreditTransfer>();
            }

            services.AddHostedService<SubmitGratsForReview>();
            services.AddHostedService<NotifyGratsApproved>();
            services.AddHostedService<NotifyGratsDenied>();
            services.AddHostedService<NotifyReviewForwarded>();
            services.AddHostedService<ImportUsers>();
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
                // TODO: This breaks ngrok, and with that, local proxying against slack. Should disable this for local development.
                // .UseHttpsRedirection()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
