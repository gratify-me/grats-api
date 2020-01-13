using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Gratify.Grats.Api.Database;
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
        private readonly GratsDb _database;
        private readonly TelemetryClient _telemetry;

        public InteractiveController(ISlackService slackService, GratsDb database, TelemetryClient telemetry)
        {
            _slackService = slackService;
            _database = database;
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
                // TODO: Validate submission, and update modal with validation messages if needed.
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
            int gratsId = -1;
            if (Interaction.ApproveGrats.Is(interaction, out gratsId))
            {
                await ApproveGrats(interaction, gratsId);
            }
            else if (Interaction.DenyGrats.Is(interaction, out gratsId))
            {
                await _slackService.ReplyToInteraction(interaction.ResponseUrl, new { text = "That's OK for now (but in the future you might have to do more to deny grats 😉)" });
            }
            else
            {
                // https://api.slack.com/reference/messaging/payload
                await _slackService.ReplyToInteraction(interaction.ResponseUrl, new { text = "Whoops! Looks like something went wrong 💩" });
            }
        }

        private async Task RequestGratsApproval(GratsViewSubmission submission)
        {
            var grats = new Grats.Api.Database.Grats
            {
                Sender = submission.User.Id,
                Content = submission.View.State.Values.GratsMessage.PlainTextInput.Value,
                Approver = submission.User.Id,
                IsApproved = false,
                Receiver = submission.View.State.Values.SelectUser.UsersSelect.SelectedUser,
            };

            await _database.Grats.AddAsync(grats);
            await _database.SaveChangesAsync();

            var channel = await _slackService.GetAppChannel(submission.User);
            var blocks = new
            {
                channel = channel.Id,
                text = $"<@{grats.Sender}> wants to send grats to <@{grats.Receiver}>!",
                blocks = new object[]
                {
                    new
                    {
                        type = "section",
                        text = new
                        {
                            type = "mrkdwn",
                            text = $"<@{grats.Sender}> wants to send grats to <@{grats.Receiver}>!",
                        },
                    },
                    new
                    {
                        type = "section",
                        text = new
                        {
                            type = "mrkdwn",
                            text = $"*Reason:*\n_{grats.Content}_",
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
                                value = $"{Interaction.ApproveGrats.Id}|{grats.Id}",
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
                                value = $"{Interaction.DenyGrats.Id}|{grats.Id}",
                            },
                        },
                    },
                },
            };

            await _slackService.SendMessage(blocks);
        }

        private async Task ApproveGrats(InteractionPayload interaction, int gratsId)
        {
            var grats = await _database.Grats.FindAsync(gratsId);
            grats.IsApproved = true;

            var channel = await _slackService.GetAppChannel(new Dto.User { Id = grats.Receiver });
            var blocks = new
            {
                channel = channel.Id,
                text = $"Congratulations! <@{grats.Sender}> just sent you grats 🎉",
                blocks = new object[]
                {
                    new
                    {
                        type = "section",
                        text = new
                        {
                            type = "mrkdwn",
                            text = $"Congratulations! <@{grats.Sender}> just sent you grats 🎉",
                        },
                    },
                    new
                    {
                        type = "section",
                        text = new
                        {
                            type = "mrkdwn",
                            text = $"*Reason:*\n_{grats.Content}_",
                        },
                    },
                    new
                    {
                        type = "section",
                        text = new
                        {
                            type = "mrkdwn",
                            text = "Would you like kr 1500;- to be transferred to your Vipps account using phone number 413 10 992?",
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
                                    text = "Yes!"
                                },
                                style = "primary",
                                value = "todo",
                            },
                            new
                            {
                                type = "button",
                                text = new
                                {
                                    type = "plain_text",
                                    emoji = true,
                                    text = "No. I would like to change my account details."
                                },
                                style = "danger",
                                value = "todo",
                            },
                        },
                    },
                },
            };

            await Task.WhenAll(new Task[]
            {
                _database.SaveChangesAsync(),
                _slackService.ReplyToInteraction(interaction.ResponseUrl, new { text = "Grats approved ✔" }),
                _slackService.SendMessage(blocks),
            });
        }
    }
}
