using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Gratify.Grats.Api.Dto;
using Gratify.Grats.Api.Services;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// https://api.slack.com/interactivity/handling
namespace Gratify.Grats.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InteractiveController : ControllerBase
    {
        private readonly ISlackService _slackService;
        private TelemetryClient _telemetry;

        public InteractiveController(ISlackService slackService, TelemetryClient telemetry)
        {
            _slackService = slackService;
            _telemetry = telemetry;
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveInteraction([FromForm(Name="payload")] string payload)
        {
            if (payload == null)
            {
                return BadRequest("Payload cannot be empty");
            }

            var json = HttpUtility.UrlDecode(payload, Encoding.UTF8);
            var type = JsonConvert.DeserializeObject<PayloadType>(json);
            if (type.IsViewSubmission)
            {
                var submission = JsonConvert.DeserializeObject<GratsViewSubmission>(json);

                _telemetry.TrackEvent("Received Submission", new Dictionary<string, string>()
                {
                    { "UserName", submission.User.Name },
                    { "Payload", payload },
                });

                await RequestGratsApproval(submission);

                return Ok();
            }

            var interaction = JsonConvert.DeserializeObject<InteractionPayload>(json);

            _telemetry.TrackEvent("Received Interaction", new Dictionary<string, string>()
            {
                { "UserName", interaction.User.Name },
                { "ResponseUrl", interaction.ResponseUrl },
                { "Payload", payload },
            });

            await HandleInteraction(interaction);

            return Ok();
        }

        private async Task HandleInteraction(InteractionPayload interaction)
        {
            if (Interaction.ApproveGrats.Is(interaction))
            {
                await Task.WhenAll(new Task[]
                {
                    _slackService.ReplyToInteraction(interaction.ResponseUrl, new { text = "Grats approved!" }),
                    _slackService.SendMessage(new
                    {
                        channel = interaction.User.Id,
                        text = $"Congratulations! @{interaction.User.Name ?? "slackbot"} just sent you grats 🎉",
                    }),
                });
            }
            else if (Interaction.DenyGrats.Is(interaction))
            {
                await _slackService.ReplyToInteraction(interaction.ResponseUrl, new { text = "That's OK for now (but in the future you might have to do more to deny grats 😉" });
            }
            else
            {
                // https://api.slack.com/reference/messaging/payload
                await _slackService.ReplyToInteraction(interaction.ResponseUrl, new { text = "Whoops! Looks like something went wrong 💩" });
            }
        }

        private async Task RequestGratsApproval(GratsViewSubmission submission)
        {
            var blocks = new
            {
                channel = submission.User.Id,
                text = $"@{submission.User.Name ?? "slackbot"} wants to send grats to @{submission.User.Name ?? "slackbot"}!",
                blocks = new object[]
                {
                    new
                    {
                        type = "section",
                        text = new
                        {
                            type = "mrkdwn",
                            text = $"@{submission.User.Name ?? "slackbot"} wants to send grats to @{submission.User.Name ?? "slackbot"}!",
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
                                    text = "Approve"
                                },
                                style = "primary",
                                value = Interaction.ApproveGrats.Id,
                            },
                            new
                            {
                                type = "button",
                                text = new
                                {
                                    type = "plain_text",
                                    emoji = true,
                                    text = "Deny"
                                },
                                style = "danger",
                                value = Interaction.DenyGrats.Id,
                            },
                        },
                    },
                },
            };

            await _slackService.SendMessage(blocks);
        }
    }
}
