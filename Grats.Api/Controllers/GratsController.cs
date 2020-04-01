using System.Threading.Tasks;
using Gratify.Grats.Api.Dto;
using Gratify.Grats.Api.Modals;
using Gratify.Grats.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gratify.Grats.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GratsController : ControllerBase
    {
        private readonly ISlackService _slackService;
        private readonly InteractionService _interactions;

        public GratsController(ISlackService slackService, InteractionService interactions)
        {
            _slackService = slackService;
            _interactions = interactions;
        }

        [HttpPost]
        public async Task<IActionResult> SendGrats([FromForm] SlashCommand slashCommand)
        {
            var sendGrats = new SendGrats(_interactions);
            var modal = sendGrats.Draw(slashCommand);

            await _slackService.OpenModal(slashCommand.TriggerId, modal);

            return Ok();
        }
    }
}
