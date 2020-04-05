using System;
using System.Threading.Tasks;
using Gratify.Api.Database.Entities;
using Gratify.Api.Dto;
using Gratify.Api.Modals;
using Gratify.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gratify.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GratsController : ControllerBase
    {
        private readonly InteractionService _interactions;
        private readonly SlackService _slackService;
        private readonly SendGrats _sendGrats;

        public GratsController(InteractionService interactions, SlackService slackService, SendGrats sendGrats)
        {
            _interactions = interactions;
            _slackService = slackService;
            _sendGrats = sendGrats;
        }

        [HttpPost]
        public async Task<IActionResult> SendGrats([FromForm] SlashCommand slashCommand)
        {
            var draft = new Draft(
                correlationId: Guid.NewGuid(),
                teamId: slashCommand.TeamId,
                createdAt: DateTime.UtcNow,
                author: slashCommand.UserId);

            await _interactions.SaveDraft(draft);
            var modal = _sendGrats.Modal(draft);
            await _slackService.OpenModal(slashCommand.TriggerId, modal);

            return Ok();
        }
    }
}
