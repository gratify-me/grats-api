using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Gratify.Api.Components;
using Gratify.Api.Components.HomeTabs;
using Gratify.Api.Components.Messages;
using Gratify.Api.Components.Modals;
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
        private readonly ComponentsService _components;

        public InteractionsController(ComponentsService components)
        {
            _components = components;
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
                var response = await _components.SendGrats.OnSubmit(submission);

                return response.Result();
            }
            else if (modalType == typeof(AllGratsSpent))
            {
                var response = await _components.AllGratsSpent.OnSubmit(submission);

                return response.Result();
            }
            else if (modalType == typeof(DenyGrats))
            {
                var response = await _components.DenyGrats.OnSubmit(submission);

                return response.Result();
            }
            else if (modalType == typeof(ForwardReview))
            {
                var response = await _components.ForwardGrats.OnSubmit(submission);

                return response.Result();
            }
            else if (modalType == typeof(RegisterAccountDetails))
            {
                var response = await _components.RegisterAccountDetails.OnSubmit(submission);

                return response.Result();
            }
            else if (modalType == typeof(AddTeamMember))
            {
                var response = await _components.AddTeamMember.OnSubmit(submission);

                return response.Result();
            }
            else if (modalType == typeof(ChangeSettings))
            {
                var response = await _components.ChangeSettings.OnSubmit(submission);

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
            if (action.ActionId.Contains(typeof(ReviewGrats).ToString()))
            {
                await _components.ReviewGrats.OnSubmit(action, triggerId);
            }
            else if (action.ActionId.Contains(typeof(ReceiveGrats).ToString()))
            {
                await _components.GratsReceived.OnSubmit(action, triggerId);
            }
            else if (action.ActionId.Contains(typeof(ShowAppHome).ToString()))
            {
                await _components.ShowAppHome.OnSubmit(action, triggerId, userId, teamId);
            }
        }
    }
}
