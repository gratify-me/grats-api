using System.Collections.Generic;
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
        public IActionResult SendGrats([FromForm] SlashCommand slashCommand)
        {
            _telemetry.TrackEvent("Received Grats", new Dictionary<string, string>()
            {
                { "UserName", slashCommand.UserName },
                { "Command", slashCommand.Command },
                { "Text", slashCommand.Text },
            });

            var blocks = new
            {
                text = $"Hi @{slashCommand.UserName ?? "slackbot"}!", // Text section is used on notifications, or other places blocks cannot be shown.
                blocks = new object[]
                {
                    new
                    {
                        type = "section",
                        text = new
                        {
                            type = "mrkdwn",
                            text = $"Hi @{slashCommand.UserName ?? "slackbot"}! Tell someone you appreciates them ❤",
                        },
                    },
                    new
                    {
                        type = "actions",
                        elements = new object[]
                        {
                            new
                            {
                                type = "button",
                                text = new
                                {
                                    type = "plain_text",
                                    emoji = true,
                                    text = "Send Grats"
                                },
                                style = "primary",
                                value = Interaction.SendGrats.Id,
                            },
                            new
                            {
                                type = "button",
                                text = new
                                {
                                    type = "plain_text",
                                    emoji = true,
                                    text = "Cancel"
                                },
                                style = "danger",
                                value = Interaction.CancelSendGrats.Id,
                            },
                        },
                    },
                },
            };

            return Ok(blocks);
        }
    }
}
