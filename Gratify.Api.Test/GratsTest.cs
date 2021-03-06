using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Gratify.Api.Test.ApiClient;
using Xunit;

namespace Gratify.Api.Test
{
    public class GratsTest : IClassFixture<GratsApiClientFactory<Startup>>, IDisposable
    {
        private readonly GratsApiClient _client;

        public GratsTest(GratsApiClientFactory<Startup> applicationFactory)
        {
            _client = applicationFactory.CreateApiClient();
        }

        [Fact]
        public async Task TestGrats()
        {
            var slackSlashCommand = new Dictionary<string, string>()
            {
                { "token", "my-test-token" },
                { "team_id", "TN27G090U" },
                { "team_domain", "project-gratify" },
                { "channel_id", "DMR7GSG11" },
                { "channel_name", "directmessage" },
                { "user_id", "UN4H54SDD" },
                { "user_name", "teodor" },
                { "command", "%2Fboble" },
                { "text", "Hi from Slack!" },
                { "response_url", "https%3A%2F%2Fhooks.slack.com%2Fcommands%2FTN27G090U%2F864672117891%2FXFvyDxvxcFFBfYLV5uZHVTmO" },
                { "trigger_id", "876149886245.750254009028.f06da6502e157c0990eb996fb2325cdc" },
            };

            var response = await _client.Grats(slackSlashCommand);
            response.Should().BeEmpty();
        }

        public void Dispose() => _client.Dispose();
    }
}
