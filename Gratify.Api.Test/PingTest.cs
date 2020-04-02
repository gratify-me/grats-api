using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Gratify.Api.Test
{
    public class PingTest : IClassFixture<WebApplicationFactory<Startup>>, IDisposable
    {
        private readonly GratsApiClient _client;

        public PingTest(WebApplicationFactory<Startup> applicationFactory)
        {
            var httpClient = applicationFactory.CreateClient();
            _client = new GratsApiClient(httpClient);
        }

        [Fact]
        public async Task Test1()
        {
            var pong = await _client.Ping();
            pong.Should().Be("PONG");
        }

        public void Dispose() => _client.Dispose();
    }
}
