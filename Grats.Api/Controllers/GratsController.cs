using System.Text.RegularExpressions;
using Gratify.Grats.Api.Dto;
using Microsoft.AspNetCore.Mvc;

// https://api.slack.com/interactivity/slash-commands
namespace Gratify.Grats.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GratsController : ControllerBase
    {
        [HttpPost]
        public string SendGrats([FromForm] SlashCommand slashCommand)
        {
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
