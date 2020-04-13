using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Gratify.Api.Messages;
using Gratify.Api.Modals;
using Microsoft.AspNetCore.Mvc;
using Slack.Client.Interactions;
using Slack.Client.Interactions.Converters;
using Slack.Client.Views;

namespace Gratify.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InteractionsController : ControllerBase
    {
        private readonly JsonSerializerOptions _options;
        private readonly SendGrats _sendGrats;
        private readonly ForwardGrats _forwardGrats;
        private readonly AddTeamMember _addTeamMember;
        private readonly RequestGratsReview _requestGratsReview;
        private readonly ShowAppHome _showAppHome;

        public InteractionsController(
            SendGrats sendGrats,
            ForwardGrats forwardGrats,
            AddTeamMember addTeamMember,
            RequestGratsReview requestGratsReview,
            ShowAppHome showAppHome)
        {
            _sendGrats = sendGrats;
            _forwardGrats = forwardGrats;
            _addTeamMember = addTeamMember;
            _requestGratsReview = requestGratsReview;
            _showAppHome = showAppHome;
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
                var response = await _sendGrats.OnSubmit(submission);

                return response.Result();
            }
            else if (modalType == typeof(ForwardGrats))
            {
                var response = await _forwardGrats.OnSubmit(submission);

                return response.Result();
            }
            else if (modalType == typeof(AddTeamMember))
            {
                var response = await _addTeamMember.OnSubmit(submission);

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
                await HandleBlockAction(action, actions.ResponseUrl, actions.TriggerId, actions.User.Id, actions.Team.Id);
            }

            return Ok();
        }

        private async Task HandleBlockAction(Slack.Client.Interactions.Action action, string responseUrl, string triggerId, string userId, string teamId)
        {
            if (action.ActionId.Contains(typeof(RequestGratsReview).ToString()))
            {
                await _requestGratsReview.OnSubmit(action, responseUrl, triggerId);
            }
            else if (action.ActionId.Contains(typeof(ShowAppHome).ToString()))
            {
                await _showAppHome.OnSubmit(action, triggerId, userId, teamId);
            }
        }
    }
}
