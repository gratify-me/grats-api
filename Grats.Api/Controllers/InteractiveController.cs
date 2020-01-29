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
                var submission = JsonConvert.DeserializeObject<GratsViewSubmission>(json);

                _telemetry.TrackEvent("Received Submission", new Dictionary<string, string>()
                {
                    { "UserName", submission.User.Name },
                    { "Payload", payload },
                });

                if (Submission.AddTeamMember.Is(submission))
                {
                    var memberId = submission.View.State.Values.SelectUser.UsersSelect.SelectedUser;
                    if (memberId == "USLACKBOT")
                    {
                        return ShowErrors();
                    }

                    var member = await _database.Users.FindAsync(memberId);
                    if (member == null)
                    {
                        member = new Database.User { Id = memberId };
                        await _database.Users.AddAsync(member);
                    }

                    var userId = submission.User.Id;
                    member.GratsApprover = submission.User.Id;
                    await _database.SaveChangesAsync();

                    var teamMembers = _database.Users.Where(user => user.GratsApprover == userId);
                    var homeBlocks = EventsController.AppHomeBlocks(teamMembers);
                    var reply = await _slackService.PublishModal(userId, homeBlocks);
                }
                else if (Submission.SendGrats.Is(submission, out var draftId))
                {
                    var draft = await _database.Drafts.FindAsync(draftId);
                    if (!draft.IsSubmitted)
                    {
                        // TODO: Could be removed, since we're not going to need this when using view.id
                        // draft.Content = submission.View.State.Values.GratsMessage.PlainTextInput.Value;
                        // draft.Receiver = submission.View.State.Values.SelectUser.UsersSelect.SelectedUser;
                        var receiver = submission.View.State.Values.SelectUser.UsersSelect.SelectedUser;
                        if (receiver == "USLACKBOT")
                        {
                            return ShowErrors();
                        }
                        else
                        {
                            draft.IsSubmitted = true;
                            await _database.SaveChangesAsync();
                            await RequestGratsApproval(submission);
                        }
                    }
                }

                return Ok(new
                {
                    response_action = "clear",
                });
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
                var grats = await _database.Grats.FindAsync(gratsId);
                if (grats.IsApproved.HasValue)
                {
                    await _slackService.ReplyToInteraction(interaction.ResponseUrl, new
                    {
                        text = "Already handled",
                        response_type = "ephemeral",
                    });
                    return;
                }

                grats.IsApproved = false;
                await _slackService.ReplyToInteraction(interaction.ResponseUrl, new
                {
                    text = "That's OK for now (but in the future you might have to do more to deny grats 😉)",
                    response_type = "ephemeral",
                });
            }
            else if (Interaction.AddTeamMember.Is(interaction))
            {
                var modal = new
                {
                    type = "modal",
                    callback_id = $"add_team_member_modal",
                    title = new
                    {
                        type = "plain_text",
                        text = "New member",
                        emoji = true,
                    },
                    submit = new
                    {
                        type = "plain_text",
                        text = "Add member",
                        emoji = true,
                    },
                    close = new
                    {
                        type = "plain_text",
                        text = "Cancel",
                        emoji = true,
                    },
                    blocks = new object[]
                    {
                        new
                        {
                            type = "input",
                            block_id = "select_user",
                            element = new
                            {
                                type = "users_select",
                                action_id = "user_selected",
                                placeholder = new
                                {
                                    type = "plain_text",
                                    text = "Select a user",
                                    emoji = true,
                                },
                            },
                            label = new
                            {
                                type = "plain_text",
                                text = ":heavy_plus_sign: Who should we add to your team?",
                                emoji = true,
                            },
                        },
                    },
                };

                var response = await _slackService.OpenModal(interaction.TriggerId, modal);
            }
            else if (Interaction.RemoveTeamMember.Is(interaction, out string memberId))
            {
                var member = await _database.Users.FindAsync(memberId);
                if (member != null)
                {
                    member.GratsApprover = null;
                    await _database.SaveChangesAsync();
                    var teamMembers = _database.Users.Where(user => user.GratsApprover == interaction.User.Id);
                    var homeBlocks = EventsController.AppHomeBlocks(teamMembers);
                    var reply = await _slackService.PublishModal(interaction.User.Id, homeBlocks);
                }
            }
            else
            {
                // https://api.slack.com/reference/messaging/payload
                await _slackService.ReplyToInteraction(interaction.ResponseUrl, new
                {
                    text = "Whoops! Looks like something went wrong 💩",
                    response_type = "ephemeral",
                });
            }
        }

        private async Task RequestGratsApproval(GratsViewSubmission submission)
        {
            var senderId = submission.User.Id;
            var approver = await _database.Users.FindAsync(senderId);
            var approverId = approver?.GratsApprover ?? senderId;
            var grats = new Grats.Api.Database.Grats
            {
                Sender = senderId,
                Content = submission.View.State.Values.GratsMessage.PlainTextInput.Value,
                Approver = approverId,
                Receiver = submission.View.State.Values.SelectUser.UsersSelect.SelectedUser,
            };

            await _database.Grats.AddAsync(grats);
            await _database.SaveChangesAsync();

            var channel = await _slackService.GetAppChannel(grats.Approver);
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
            if (grats.IsApproved.HasValue)
            {
                await _slackService.ReplyToInteraction(interaction.ResponseUrl, new { text = "Already handled" });
                return;
            }

            grats.IsApproved = true;

            var channel = await _slackService.GetAppChannel(grats.Receiver);
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
                _slackService.ReplyToInteraction(interaction.ResponseUrl, new
                {
                    text = "Grats approved ✔",
                    response_type = "ephemeral",
                }),
                _slackService.SendMessage(blocks),
            });
        }

        private IActionResult ShowErrors()
        {
            var errors = new
            {
                response_action = "errors",
                errors = new
                {
                    select_user = $"Slackbot is not a valid user", // select_user is block_id of element with error.
                },
            };

            return Ok(errors);
        }

        // Reply can also post a completely new modal if needed.
        // Can also fill inputs with data from custom providers and more.
        private IActionResult ReplyInvalidGrats(Draft draft)
        {
            var modal = new
            {
                type = "modal",
                callback_id = $"send-grats-modal|{draft.Id}",
                title = new
                {
                    type = "plain_text",
                    text = "Send Grats",
                    emoji = true,
                },
                submit = new
                {
                    type = "plain_text",
                    text = "Send Grats",
                    emoji = true,
                },
                close = new
                {
                    type = "plain_text",
                    text = "Cancel",
                    emoji = true,
                },
                blocks = new object[]
                {
                    new
                    {
                        type = "input",
                        block_id = "select_user",
                        element = new
                        {
                            type = "users_select",
                            action_id = "user_selected",
                            placeholder = new
                            {
                                type = "plain_text",
                                text = "Select a user",
                                emoji = true,
                            },
                        },
                        label = new
                        {
                            type = "plain_text",
                            text = "Who should receive Grats?",
                            emoji = true,
                        },
                    },
                    new
                    {
                        type = "context",
                        elements = new object[]
                        {
                            new
                            {
                                type = "mrkdwn",
                                text = $":heavy_multiplication_x: Cannot send grats to <@{draft.Receiver}>",
                            },
                        },
                    },
                    new
                    {
                        type = "input",
                        block_id = "grats_message",
                        element = new
                        {
                            type = "plain_text_input",
                            action_id = "grats_message_written",
                            multiline = true,
                            placeholder = new
                            {
                                type = "plain_text",
                                text = "A short and concrete description",
                                emoji = true,
                            },
                        },
                        label = new
                        {
                            type = "plain_text",
                            text = "Why should they receive Grats?",
                            emoji = true,
                        },
                    },
                },
            };

            var response = new
            {
                response_action = "update",
                view = modal,
            };

            return Ok(response);
        }
    }
}
