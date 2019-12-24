using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Gratify.Grats.Api.Test
{
    public class InteractiveTest : IClassFixture<WebApplicationFactory<Startup>>, IDisposable
    {
        private readonly GratsApiClient _client;

        public InteractiveTest(WebApplicationFactory<Startup> applicationFactory)
        {
            var httpClient = applicationFactory.CreateClient();
            _client = new GratsApiClient(httpClient);
        }

        [Fact]
        public async Task TestGrats()
        {
            var slackInteractivePayload = new Dictionary<string, string>()
            {
                { "payload", "%7B%22type%22%3A%22block_actions%22%2C%22team%22%3A%7B%22id%22%3A%22TN27G090U%22%2C%22domain%22%3A%22project-gratify%22%7D%2C%22user%22%3A%7B%22id%22%3A%22UN4H54SDD%22%2C%22username%22%3A%22teodor%22%2C%22name%22%3A%22teodor%22%2C%22team_id%22%3A%22TN27G090U%22%7D%2C%22api_app_id%22%3A%22AQDV9E4HE%22%2C%22token%22%3A%22my-test-token%22%2C%22container%22%3A%7B%22type%22%3A%22message%22%2C%22message_ts%22%3A%221576622376.000100%22%2C%22channel_id%22%3A%22DMR7GSG11%22%2C%22is_ephemeral%22%3Atrue%7D%2C%22trigger_id%22%3A%22877822948581.750254009028.ac695f1c6f5c2fdc1952bedb289ad892%22%2C%22channel%22%3A%7B%22id%22%3A%22DMR7GSG11%22%2C%22name%22%3A%22directmessage%22%7D%2C%22response_url%22%3A%22https%3A%5C%2F%5C%2Fhooks.slack.com%5C%2Factions%5C%2FTN27G090U%5C%2F866345325155%5C%2Fky1Ky7qZf9BdgFE76i2esq4U%22%2C%22actions%22%3A%5B%7B%22action_id%22%3A%22tkvbr%22%2C%22block_id%22%3A%22IJ9%22%2C%22text%22%3A%7B%22type%22%3A%22plain_text%22%2C%22text%22%3A%22Approve%22%2C%22emoji%22%3Atrue%7D%2C%22value%22%3A%22click_me_123%22%2C%22style%22%3A%22primary%22%2C%22type%22%3A%22button%22%2C%22action_ts%22%3A%221576622378.642801%22%7D%5D%7D" },
            };

            var response = await _client.Interactive(slackInteractivePayload);
            response.Should().BeEmpty();
        }

        public void Dispose() => _client.Dispose();
    }
}
