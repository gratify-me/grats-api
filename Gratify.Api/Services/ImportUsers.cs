using System;
using System.Threading;
using System.Threading.Tasks;
using Gratify.Api.Database;
using Gratify.Api.Database.Entities;
using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Slack.Client;

namespace Gratify.Api.Services
{
    public class ImportUsers : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly TelemetryClient _telemetry;
        private readonly SlackService _slackService;

        public ImportUsers(IServiceProvider services, TelemetryClient telemetry, SlackService slackService)
        {
            _services = services;
            _telemetry = telemetry;
            _slackService = slackService;
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                using var scope = _services.CreateScope();
                var database = scope.ServiceProvider.GetRequiredService<GratsDb>();
                var slackUsers = _slackService.GetUsers();

                await foreach (var slackUser in slackUsers)
                {
                    var user = await database.Users.SingleOrDefaultAsync(user => user.UserId == slackUser.Id);
                    if (user == default && slackUser.Deleted)
                    {
                        continue;
                    }
                    else if (user != default && slackUser.Deleted)
                    {
                        database.Users.Remove(user);
                        await database.SaveChangesAsync();
                    }
                    else if (user == default)
                    {
                        var newUser = new User(
                            teamId: slackUser.TeamId,
                            userId: slackUser.Id,
                            isEligibleForGrats: IsEligibleForGrats(slackUser),
                            isAdministrator: IsAdministrator(slackUser),
                            updatedAt: slackUser.Updated);

                        await database.Users.AddAsync(newUser);
                        await database.SaveChangesAsync();
                    }
                    else if (user.UpdatedAt < slackUser.Updated)
                    {
                        user.IsEligibleForGrats = IsEligibleForGrats(slackUser);
                        user.IsAdministrator = IsAdministrator(slackUser);
                        user.UpdatedAt = slackUser.Updated;
                        await database.SaveChangesAsync();
                    }
                }

                // users.list is rate limited at 20 requests per minute.
                // Since every call to _slackService.GetUsers() potentially
                // creates more than one request, we shouldn't make this delay to low.
                // https://api.slack.com/docs/rate-limits#tier_t2
                await Task.Delay(320000, token);
            }
        }

        private bool IsEligibleForGrats(Slack.Client.Primitives.User slackUser) =>
            slackUser.Id != "USLACKBOT" && // slackbot has IsBot = true for some reason.
            !slackUser.IsBot &&
            !slackUser.IsRestricted &&
            !slackUser.IsUltraRestricted;

        private bool IsAdministrator(Slack.Client.Primitives.User slackUser) =>
            slackUser.IsAdmin ||
            slackUser.IsOwner ||
            slackUser.IsPrimaryOwner;
    }
}