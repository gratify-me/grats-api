using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Gratify.Grats.Api.Database;
using Gratify.Grats.Api.GratsActions;
using Gratify.Grats.Api.Messages;
using Gratify.Grats.Api.Modals;
using Gratify.Grats.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Slack.Client.Chat;
using Slack.Client.Events;
using Slack.Client.Interactions;
using Slack.Client.Interactions.Converters;
using Slack.Client.Views;

namespace Gratify.Grats.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InteractiveController : ControllerBase
    {
        private readonly ISlackService _slackService;
        private readonly InteractionService _interactions;
        private readonly GratsDb _database;
        private readonly JsonSerializerOptions _options;

        public InteractiveController(ISlackService slackService, InteractionService interactions, GratsDb database)
        {
            _slackService = slackService;
            _interactions = interactions;
            _database = database;
            _options = new JsonSerializerOptions
            {
                IgnoreNullValues = true
            };

            _options.Converters.Add(new InteractionPayloadConverter());
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveInteraction([FromForm(Name="payload")] string payload)
        {
            if (payload == null)
            {
                return BadRequest("Payload cannot be empty");
            }

            var json = HttpUtility.UrlDecode(payload, Encoding.UTF8);
            var interactionPayload = JsonSerializer.Deserialize<InteractionPayload>(json);

            return interactionPayload switch
            {
                ViewSubmission submission => await HandleViewSubmission(submission),
                BlockActions actions => await HandleBlockActions(actions),
                _ => Ok(),
            };
        }

        public async Task<IActionResult> HandleViewSubmission(ViewSubmission submission)
        {
            var modalType = Type.GetType(submission.View.CallbackId);
            if (modalType == typeof(SendGrats))
            {
                var sendGrats = new SendGrats(_interactions);
                var response = await sendGrats.OnSubmit(submission);

                return response.Result();
            }
            else if (modalType == typeof(Modals.ForwardGrats))
            {
                var forwardGrats = new Modals.ForwardGrats(_interactions);
                var response = await forwardGrats.OnSubmit(submission);

                return response.Result();
            }
            else if (modalType == typeof(Modals.AddTeamMember))
            {
                var addTeamMember = new Modals.AddTeamMember(_database, _interactions);
                var response = await addTeamMember.OnSubmit(submission);

                return response.Result();
            }
            else
            {
                return new ResponseActionClear().Result();
            }
        }

        private async Task<IActionResult> HandleBlockActions(BlockActions actions)
        {
            foreach (var action in actions.Actions)
            {
                await HandleBlockAction(actions, action);
            }

            return Ok();
        }

        private async Task HandleBlockAction(BlockActions actions, Slack.Client.Interactions.Action action)
        {
            // TODO: Pass values from actions.
            int gratsId = -1;
            if (action.Value == ApproveGrats.Name)
            {
                var grats = await _database.Grats.FindAsync(gratsId);
                if (grats.IsApproved.HasValue)
                {
                    await _slackService.ReplyToInteraction(actions.ResponseUrl, new MessagePayload { Text = "Already handled" });
                    return;
                }

                grats.IsApproved = true;

                var channel = await _slackService.GetAppChannel(grats.Receiver);
                var gratsApproved = new GratsApprovedMessage(grats, channel);
                var blocks = gratsApproved.Draw();

                await Task.WhenAll(new Task[]
                {
                    _database.SaveChangesAsync(),
                    _slackService.ReplyToInteraction(actions.ResponseUrl, new ResponseMessage
                    {
                        Text = "Grats approved ✔",
                        ResponseType = "ephemeral",
                    }),
                    _slackService.SendMessage(blocks),
                });

                return;
            }
            else if (action.Value == GratsActions.ForwardGrats.Name)
            {
                var forwardGrats = new Modals.ForwardGrats(_interactions);
                var modal = forwardGrats.Draw(actions);
                await _slackService.OpenModal(actions.TriggerId, modal);

                return;
            }
            else if (action.Value == DenyGrats.Name)
            {
                var grats = await _database.Grats.FindAsync(gratsId);
                if (grats.IsApproved.HasValue)
                {
                    await _slackService.ReplyToInteraction(actions.ResponseUrl, new ResponseMessage
                    {
                        Text = "Already handled",
                        ResponseType = "ephemeral",
                    });

                    return;
                }

                grats.IsApproved = false;
                var message = new OkForNowMessage().Draw();
                await _slackService.ReplyToInteraction(actions.ResponseUrl, message);

                return;
            }
            else if (action.Value == GratsActions.AddTeamMember.Name)
            {
                var addTeamMember = new Modals.AddTeamMember(_database, _interactions);
                var modal = addTeamMember.Draw(actions);
                var response = await _slackService.OpenModal(actions.TriggerId, modal);

                return;
            }
            else if (action.Value == RemoveTeamMember.Name)
            {
                // TODO: Pass values from actions.
                var memberId = -1;
                var member = await _database.Users.FindAsync(memberId);
                if (member != null)
                {
                    member.GratsApprover = null;
                    await _database.SaveChangesAsync();

                    var appHome = new ShowAppHome(_database, _interactions);
                    var appHomeOpened = new AppHomeOpened
                    {
                        User = actions.User.Id
                    };
                    var homeBlocks = await appHome.Draw(appHomeOpened);
                    var reply = await _slackService.PublishModal(actions.User.Id, homeBlocks);
                }

                return;
            }
            else
            {
                var message = new ErrorMessage().Draw();
                await _slackService.ReplyToInteraction(actions.ResponseUrl, message);

                return;
            }
        }
    }
}
