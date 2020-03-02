using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Gratify.Grats.Api.Database;
using Gratify.Grats.Api.Services;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Slack.Client.BlockKit.BlockElements;
using Slack.Client.BlockKit.BlockElements.Selects;
using Slack.Client.BlockKit.CompositionObjects;
using Slack.Client.BlockKit.LayoutBlocks;
using Slack.Client.Chat;
using Slack.Client.Views;

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
            var type = JsonSerializer.Deserialize<Dto.PayloadType>(json);
            if (type.IsViewSubmission)
            {
                var submission = JsonSerializer.Deserialize<Dto.GratsViewSubmission>(json);

                _telemetry.TrackEvent("Received Submission", new Dictionary<string, string>()
                {
                    { "UserName", submission.User.Name },
                    { "Payload", payload },
                });

                if (Dto.Submission.AddTeamMember.Is(submission))
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
                else if (Dto.Submission.ForwardGrats.Is(submission, out var gratsId))
                {
                    var newApprover = submission.View.State.Values.SelectUser.UsersSelect.SelectedUser;
                    if (newApprover == "USLACKBOT")
                    {
                        return ShowErrors();
                    }
                    else
                    {
                        await SubmitForwardGrats(submission, gratsId, newApprover);
                    }
                }
                else if (Dto.Submission.SendGrats.Is(submission, out var draftId))
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

            var interaction = JsonSerializer.Deserialize<Dto.InteractionPayload>(json);

            _telemetry.TrackEvent("Received Interaction", new Dictionary<string, string>()
            {
                { "UserName", interaction.User.Name },
                { "ResponseUrl", interaction.ResponseUrl },
                { "Payload", payload },
            });

            await HandleInteraction(interaction);

            return Ok();
        }

        private async Task HandleInteraction(Dto.InteractionPayload interaction)
        {
            int gratsId = -1;
            if (Dto.Interaction.ApproveGrats.Is(interaction, out gratsId))
            {
                await ApproveGrats(interaction, gratsId);
            }
            else if (Dto.Interaction.ForwardGrats.Is(interaction, out gratsId))
            {
                await ForwardGrats(interaction, gratsId);
            }
            else if (Dto.Interaction.DenyGrats.Is(interaction, out gratsId))
            {
                var grats = await _database.Grats.FindAsync(gratsId);
                if (grats.IsApproved.HasValue)
                {
                    await _slackService.ReplyToInteraction(interaction.ResponseUrl, new ResponseMessage
                    {
                        Text = "Already handled",
                        ResponseType = "ephemeral",
                    });
                    return;
                }

                grats.IsApproved = false;
                await _slackService.ReplyToInteraction(interaction.ResponseUrl, new ResponseMessage
                {
                    Text = "That's OK for now (but in the future you might have to do more to deny grats 😉)",
                    ResponseType = "ephemeral",
                });
            }
            else if (Dto.Interaction.AddTeamMember.Is(interaction))
            {
                var modal = new Modal
                {
                    CallbackId = $"add_team_member_modal",
                    Title = new PlainText
                    {
                        Text = "New member",
                        Emoji = true,
                    },
                    Submit = new PlainText
                    {
                        Text = "Add member",
                        Emoji = true,
                    },
                    Close = new PlainText
                    {
                        Text = "Cancel",
                        Emoji = true,
                    },
                    Blocks = new LayoutBlock[]
                    {
                        new Input
                        {
                            BlockId = "select_user",
                            Element = new UsersSelect
                            {
                                ActionId = "user_selected",
                                Placeholder = new PlainText
                                {
                                    Text = "Select a user",
                                    Emoji = true,
                                },
                            },
                            Label = new PlainText
                            {
                                Text = ":heavy_plus_sign: Who should we add to your team?",
                                Emoji = true,
                            },
                        },
                    },
                };

                var response = await _slackService.OpenModal(interaction.TriggerId, modal);
            }
            else if (Dto.Interaction.RemoveTeamMember.Is(interaction, out string memberId))
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
                await _slackService.ReplyToInteraction(interaction.ResponseUrl, new ResponseMessage
                {
                    Text = "Whoops! Looks like something went wrong 💩",
                    ResponseType = "ephemeral",
                });
            }
        }

        private async Task SubmitForwardGrats(Dto.GratsViewSubmission submission, int gratsId, string newApprover)
        {
            var grats = await _database.Grats.FindAsync(gratsId);
            grats.Approver = newApprover;

            var channel = await _slackService.GetAppChannel(grats.Approver);
            var blocks = new PostMessage
            {
                Channel = channel.Id,
                Text = $"<@{grats.Sender}> wants to send grats to <@{grats.Receiver}>!",
                Blocks = new LayoutBlock[]
                {
                    new Section
                    {
                        Text = new MrkdwnText
                        {
                            Text = $"<@{grats.Sender}> wants to send grats to <@{grats.Receiver}>!",
                        },
                    },
                    new Section
                    {
                        Text = new MrkdwnText
                        {
                            Text = $"*Reason:*\n_{grats.Content}_",
                        },
                    },
                    new Actions
                    {
                        Elements = new Button[]
                        {
                            new Button
                            {
                                Text = new PlainText
                                {
                                    Text = "Approve",
                                    Emoji = true,
                                },
                                Style = "primary",
                                Value = $"{Dto.Interaction.ApproveGrats.Id}|{grats.Id}",
                            },
                            new Button
                            {
                                Text = new PlainText
                                {
                                    Text = "Deny",
                                    Emoji = true,
                                },
                                Style = "danger",
                                Value = $"{Dto.Interaction.DenyGrats.Id}|{grats.Id}",
                            },
                            new Button
                            {
                                Text = new PlainText
                                {
                                    Text = "Forward",
                                    Emoji = true,
                                },
                                Value = $"{Dto.Interaction.ForwardGrats.Id}|{grats.Id}",
                            },
                        },
                    },
                },
            };

            await _slackService.SendMessage(blocks);
        }

        private async Task RequestGratsApproval(Dto.GratsViewSubmission submission)
        {
            var senderId = submission.User.Id;
            var approver = await _database.Users.FindAsync(senderId);
            var approverId = approver?.GratsApprover ?? senderId;
            var grats = new Database.Grats
            {
                Sender = senderId,
                Content = submission.View.State.Values.GratsMessage.PlainTextInput.Value,
                Approver = approverId,
                Receiver = submission.View.State.Values.SelectUser.UsersSelect.SelectedUser,
            };

            await _database.Grats.AddAsync(grats);
            await _database.SaveChangesAsync();

            var channel = await _slackService.GetAppChannel(grats.Approver);
            var blocks = new PostMessage
            {
                Channel = channel.Id,
                Text = $"<@{grats.Sender}> wants to send grats to <@{grats.Receiver}>!",
                Blocks = new LayoutBlock[]
                {
                    new Section
                    {
                        Text = new MrkdwnText
                        {
                            Text = $"<@{grats.Sender}> wants to send grats to <@{grats.Receiver}>!",
                        },
                    },
                    new Section
                    {
                        Text = new MrkdwnText
                        {
                            Text = $"*Reason:*\n_{grats.Content}_",
                        },
                    },
                    new Actions
                    {
                        Elements = new Button[]
                        {
                            new Button
                            {
                                Text = new PlainText
                                {
                                    Text = "Approve",
                                    Emoji = true,
                                },
                                Style = "primary",
                                Value = $"{Dto.Interaction.ApproveGrats.Id}|{grats.Id}",
                            },
                            new Button
                            {
                                Text = new PlainText
                                {
                                    Text = "Deny",
                                    Emoji = true,
                                },
                                Style = "danger",
                                Value = $"{Dto.Interaction.DenyGrats.Id}|{grats.Id}",
                            },
                            new Button
                            {
                                Text = new PlainText
                                {
                                    Text = "Forward",
                                    Emoji = true,
                                },
                                Value = $"{Dto.Interaction.ForwardGrats.Id}|{grats.Id}",
                            },
                        },
                    },
                },
            };

            await _slackService.SendMessage(blocks);
        }

        private async Task ForwardGrats(Dto.InteractionPayload interaction, int gratsId)
        {
            var modal = new Modal
            {
                CallbackId = $"forward-grats-modal|{gratsId}",
                Title = new PlainText
                {
                    Text = "Forward Grats",
                    Emoji = true,
                },
                Submit = new PlainText
                {
                    Text = "Forward Grats",
                    Emoji = true,
                },
                Close = new PlainText
                {
                    Text = "Cancel",
                    Emoji = true,
                },
                Blocks = new LayoutBlock[]
                {
                    new Input
                    {
                        BlockId = "select_user",
                        Element = new UsersSelect
                        {
                            ActionId = "user_selected",
                            Placeholder = new PlainText
                            {
                                Text = "Select a user",
                                Emoji = true,
                            },
                        },
                        Label = new PlainText
                        {
                            Text = "Who should approve Grats?",
                            Emoji = true,
                        },
                    },
                    new Section
                    {
                        Text = new PlainText
                        {
                            Text = "Transfer approval responsibility permanently",
                        },
                        Accessory = new RadioButtonGroup
                        {
                            ActionId = "should_transfer_approval_responsibility",
                            InitialOption = new Option
                            {
                                Value = "No",
                                Text = new PlainText { Text = "No" }
                            },
                            Options = new Option[]
                            {
                                new Option
                                {
                                    Value = "Yes",
                                    Text = new PlainText { Text = "Yes" }
                                },
                                new Option
                                {
                                    Value = "No",
                                    Text = new PlainText { Text = "No" },
                                },
                            },
                        },
                    },
                },
            };

            await _slackService.OpenModal(interaction.TriggerId, modal);
        }

        private async Task ApproveGrats(Dto.InteractionPayload interaction, int gratsId)
        {
            var grats = await _database.Grats.FindAsync(gratsId);
            if (grats.IsApproved.HasValue)
            {
                await _slackService.ReplyToInteraction(interaction.ResponseUrl, new MessagePayload { Text = "Already handled" });
                return;
            }

            grats.IsApproved = true;

            var channel = await _slackService.GetAppChannel(grats.Receiver);
            var blocks = new PostMessage
            {
                Channel = channel.Id,
                Text = $"Congratulations! <@{grats.Sender}> just sent you grats 🎉",
                Blocks = new LayoutBlock[]
                {
                    new Section
                    {
                        Text = new MrkdwnText
                        {
                            Text = $"Congratulations! <@{grats.Sender}> just sent you grats 🎉",
                        },
                    },
                    new Section
                    {
                        Text = new MrkdwnText
                        {
                            Text = $"*Reason:*\n_{grats.Content}_",
                        },
                    },
                    new Section
                    {
                        Text = new MrkdwnText
                        {
                            Text = "Would you like kr 1500;- to be transferred to your Vipps account using phone number 413 10 992?",
                        },
                    },
                    new Actions
                    {
                        Elements = new Button[]
                        {
                            new Button
                            {
                                Text = new PlainText
                                {
                                    Text = "Yes!",
                                    Emoji = true,
                                },
                                Style = "primary",
                                Value = "todo",
                            },
                            new Button
                            {
                                Text = new PlainText
                                {
                                    Text = "No. I would like to change my account details.",
                                    Emoji = true,
                                },
                                Style = "danger",
                                Value = "todo",
                            },
                        },
                    },
                },
            };

            await Task.WhenAll(new Task[]
            {
                _database.SaveChangesAsync(),
                _slackService.ReplyToInteraction(interaction.ResponseUrl, new ResponseMessage
                {
                    Text = "Grats approved ✔",
                    ResponseType = "ephemeral",
                }),
                _slackService.SendMessage(blocks),
            });
        }

        private IActionResult ShowErrors()
        {
            var errors = new ResponseActionErrors
            {
                Errors = new Dictionary<string, string>()
                {
                    // select_user is block_id of element with error.
                    { "select_user", $"Slackbot is not a valid user" },
                },
            };

            return Ok(errors);
        }

        // Reply can also post a completely new modal if needed.
        // Can also fill inputs with data from custom providers and more.
        private IActionResult ReplyInvalidGrats(Draft draft)
        {
            var modal = new Modal
            {
                CallbackId = $"send-grats-modal|{draft.Id}",
                Title = new PlainText
                {
                    Text = "Send Grats",
                    Emoji = true,
                },
                Submit = new PlainText
                {
                    Text = "Send Grats",
                    Emoji = true,
                },
                Close = new PlainText
                {
                    Text = "Cancel",
                    Emoji = true,
                },
                Blocks = new LayoutBlock[]
                {
                    new Input
                    {
                        BlockId = "select_user",
                        Element = new UsersSelect
                        {
                            ActionId = "user_selected",
                            Placeholder = new PlainText
                            {
                                Text = "Select a user",
                                Emoji = true,
                            },
                        },
                        Label = new PlainText
                        {
                            Text = "Who should receive Grats?",
                            Emoji = true,
                        },
                    },
                    new Context
                    {
                        Elements = new MrkdwnText[]
                        {
                            new MrkdwnText
                            {
                                Text = $":heavy_multiplication_x: Cannot send grats to <@{draft.Receiver}>",
                            },
                        },
                    },
                    new Input
                    {
                        BlockId = "grats_message",
                        Element = new PlainTextInput
                        {
                            ActionId = "grats_message_written",
                            Multiline = true,
                            Placeholder = new PlainText
                            {
                                Text = "A short and concrete description",
                                Emoji = true,
                            },
                        },
                        Label = new PlainText
                        {
                            Text = "Why should they receive Grats?",
                            Emoji = true,
                        },
                    },
                },
            };

            var response = new ResponseActionUpdate
            {
                View = modal,
            };

            return Ok(response);
        }
    }
}
