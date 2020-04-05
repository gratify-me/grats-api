using System.Threading.Tasks;
using Gratify.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Slack.Client.Events;

namespace Gratify.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly InteractionService _interactions;

        public EventsController(InteractionService interactions)
        {
            _interactions = interactions;
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveEvent([FromBody] EventWrapper slackEvent) =>
            slackEvent switch
            {
                UrlVerificationRequest request => Ok(new UrlVerificationResponse(request)),
                EventCallback request => request.Event switch
                {
                    AppHomeOpened appHomeOpened => await ShowAppHome(appHomeOpened),
                    _ => Ok()
                },
                _ => Ok()
            };

        private async Task<IActionResult> ShowAppHome(AppHomeOpened appHomeOpened)
        {
            await _interactions.ShowAppHome(appHomeOpened.User);

            return Ok();
        }
    }
}
