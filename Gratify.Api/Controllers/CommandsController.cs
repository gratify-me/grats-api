using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gratify.Api.Commands;
using Gratify.Api.Components;
using Gratify.Api.Database.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Gratify.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommandsController : ControllerBase
    {
        private readonly Regex _userIdRegex = new Regex(@"(?<=<@)([A-Z0-9])+(?=\|.+>)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly ComponentsService _components;

        public CommandsController(ComponentsService components)
        {
            _components = components;
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveCommand([FromForm] SlashCommand slashCommand) =>
            slashCommand.Command switch
            {
                "/grats" => await SendGrats(slashCommand),
                _ => Ok(),
            };

        private async Task<IActionResult> SendGrats(SlashCommand slashCommand)
        {
            var authorId = slashCommand.UserId;
            var userId = GetUserId(slashCommand);
            await _components.SendGrats.Open(slashCommand.TriggerId, slashCommand.TeamId, authorId, userId);

            return Ok();
        }

        private string GetUserId(SlashCommand slashCommand)
        {
            if (string.IsNullOrEmpty(slashCommand.Text))
            {
                return null;
            }

            if (!_userIdRegex.IsMatch(slashCommand.Text))
            {
                return null;
            }

            var match = _userIdRegex.Match(slashCommand.Text);
            return match.ToString();
        }
    }
}
