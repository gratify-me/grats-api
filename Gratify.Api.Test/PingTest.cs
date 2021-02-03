using System;
using System.Threading.Tasks;
using FluentAssertions;
using Gratify.Api.Test.ApiClient;
using Xunit;

namespace Gratify.Api.Test
{
    public class PingTest : IClassFixture<GratsApiClientFactory<Startup>>, IDisposable
    {
        private readonly GratsApiClient _client;

        public PingTest(GratsApiClientFactory<Startup> applicationFactory)
        {
            _client = applicationFactory.CreateApiClient();
        }

        [Fact]
        public async Task ShouldReturnPong()
        {
            var pong = await _client.Ping();
            pong.Should().Be("PONG");
        }

        public void Dispose() => _client.Dispose();
    }
}
