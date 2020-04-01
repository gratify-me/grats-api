using System.Threading.Tasks;
using Gratify.Grats.Api.Database;
using Gratify.Grats.Api.Modals;
using Gratify.Grats.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Slack.Client.Events;

namespace Gratify.Grats.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly ISlackService _slackService;
        private readonly InteractionService _interactions;
        private readonly GratsDb _database;

        public EventsController(ISlackService slackService, InteractionService interactions, GratsDb database)
        {
            _slackService = slackService;
            _interactions = interactions;
            _database = database;
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
            var appHome = new ShowAppHome(_database, _interactions);
            var homeBlocks = await appHome.Draw(appHomeOpened);
            var userId = appHomeOpened.User;

            await _slackService.PublishModal(userId, homeBlocks);

            return Ok();
        }
    }
}
