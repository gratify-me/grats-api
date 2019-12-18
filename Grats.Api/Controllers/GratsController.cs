using System.Collections.Generic;
using System.Text.RegularExpressions;
using Gratify.Grats.Api.Dto;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

// https://api.slack.com/interactivity/slash-commands
namespace Gratify.Grats.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GratsController : ControllerBase
    {
        private TelemetryClient _telemetry;

        public GratsController(TelemetryClient telemetry)
        {
            _telemetry = telemetry;
        }

        [HttpPost]
        public string SendGrats([FromForm] SlashCommand slashCommand)
        {
            _telemetry.TrackEvent("Received Grats", new Dictionary<string, string>()
            {
                { "UserName", slashCommand.UserName },
                { "Command", slashCommand.Command },
                { "Text", slashCommand.Text },
            });

            var blocks = @"
            {
                ""blocks"": [
                    {
                        ""type"": ""section"",
                        ""text"": {
                            ""type"": ""mrkdwn"",
                            ""text"": ""Hi @slashCommand.UserName! Tell someone you appreciates them!""
                        }
                    },
                    {
                        ""type"": ""actions"",
                        ""elements"": [
                            {
                                ""type"": ""button"",
                                ""text"": {
                                    ""type"": ""plain_text"",
                                    ""emoji"": true,
                                    ""text"": ""Send Grats""
                                },
                                ""style"": ""primary"",
                                ""value"": ""send_grats""
                            },
                            {
                                ""type"": ""button"",
                                ""text"": {
                                    ""type"": ""plain_text"",
                                    ""emoji"": true,
                                    ""text"": ""Cancel""
                                },
                                ""style"": ""danger"",
                                ""value"": ""cancel_send_grats""
                            }
                        ]
                    }
                ]
            }";

            return Regex.Replace(blocks, "slashCommand.UserName", slashCommand.UserName);
        }
    }
}
