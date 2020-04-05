using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Gratify.Api.Database;
using Gratify.Api.Messages;
using Gratify.Api.Modals;
using Gratify.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Slack.Client.Interactions;
using Slack.Client.Interactions.Converters;
using Slack.Client.Views;

namespace Gratify.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InteractiveController : ControllerBase
    {
        private readonly InteractionService _interactions;
        private readonly GratsDb _database;
        private readonly JsonSerializerOptions _options;

        public InteractiveController(InteractionService interactions, GratsDb database)
        {
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
            var interactionPayload = JsonSerializer.Deserialize<InteractionPayload>(json, _options);

            return interactionPayload switch
            {
                ViewSubmission submission => await HandleViewSubmission(submission),
                BlockActions actions => await HandleBlockActions(actions),
                _ => Ok(),
            };
        }

        private async Task<IActionResult> HandleViewSubmission(ViewSubmission submission)
        {
            var modalType = Type.GetType(submission.View.CallbackId);
            if (modalType == typeof(SendGrats))
            {
                var sendGrats = new SendGrats(_interactions);
                var response = await sendGrats.OnSubmit(submission);

                return response.Result();
            }
            else if (modalType == typeof(ForwardGrats))
            {
                var forwardGrats = new ForwardGrats(_interactions);
                var response = await forwardGrats.OnSubmit(submission);

                return response.Result();
            }
            else if (modalType == typeof(AddTeamMember))
            {
                var addTeamMember = new AddTeamMember(_interactions);
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
                await HandleBlockAction(action, actions.ResponseUrl, actions.TriggerId, actions.User.Id);
            }

            return Ok();
        }

        private async Task HandleBlockAction(Slack.Client.Interactions.Action action, string responseUrl, string triggerId, string userId)
        {
            if (action.ActionId.Contains(typeof(RequestGratsReview).ToString()))
            {
                var requestGratsReview = new RequestGratsReview(_interactions);
                await requestGratsReview.OnSubmit(action, responseUrl, triggerId);
            }
            else if (action.ActionId.Contains(typeof(ShowAppHome).ToString()))
            {
                var showAppHome = new ShowAppHome(_interactions, _database);
                await showAppHome.OnSubmit(action, triggerId, userId);
            }
        }
    }
}
