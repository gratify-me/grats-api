﻿using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gratify.Api.Database.Entities;
using Gratify.Api.Dto;
using Gratify.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gratify.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommandsController : ControllerBase
    {
        private readonly Regex _userIdRegex = new Regex(@"(?<=<@)([A-Z0-9])+(?=\|\w+>)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly InteractionService _interactions;

        public CommandsController(InteractionService interactions)
        {
            _interactions = interactions;
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
            var draft = new Draft(
                correlationId: Guid.NewGuid(),
                teamId: slashCommand.TeamId,
                createdAt: DateTime.UtcNow,
                author: slashCommand.UserId);

            var userId = GetUserId(slashCommand);
            await _interactions.SendGrats(draft, slashCommand.TriggerId, userId);

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