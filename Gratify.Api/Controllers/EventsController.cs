using System.Threading.Tasks;
using Gratify.Api.Components;
using Microsoft.AspNetCore.Mvc;
using Slack.Client.Events;

namespace Gratify.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly ComponentsService _components;

        public EventsController(ComponentsService components)
        {
            _components = components;
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveEvent([FromBody] EventWrapper slackEvent) =>
            slackEvent switch
            {
                UrlVerificationRequest request => Ok(new UrlVerificationResponse(request)),
                EventCallback request => request.Event switch
                {
                    AppHomeOpened appHomeOpened => await ShowAppHome(request.TeamId, appHomeOpened.User),
                    _ => Ok()
                },
                _ => Ok()
            };

        private async Task<IActionResult> ShowAppHome(string teamId, string userId)
        {
            await _components.ShowAppHome.DisplayFor(teamId, userId);

            return Ok();
        }
    }
}
