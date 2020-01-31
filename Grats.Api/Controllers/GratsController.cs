using System.Collections.Generic;
using System.Threading.Tasks;
using Gratify.Grats.Api.Database;
using Gratify.Grats.Api.Dto;
using Gratify.Grats.Api.Services;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

// https://api.slack.com/interactivity/slash-commands
namespace Gratify.Grats.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GratsController : ControllerBase
    {
        private readonly ISlackService _slackService;
        private readonly GratsDb _database;
        private readonly TelemetryClient _telemetry;

        public GratsController(ISlackService slackService, GratsDb database, TelemetryClient telemetry)
        {
            _slackService = slackService;
            _database = database;
            _telemetry = telemetry;
        }

        [HttpPost]
        public async Task<IActionResult> SendGrats([FromForm] SlashCommand slashCommand)
        {
            _telemetry.TrackEvent("Received Grats", new Dictionary<string, string>()
            {
                { "UserName", slashCommand.UserName },
                { "Command", slashCommand.Command },
                { "Text", slashCommand.Text },
            });

            var draft = new Draft
            {
                Sender = slashCommand.UserId,
                IsSubmitted = false,
            };

            await _database.Drafts.AddAsync(draft);
            await _database.SaveChangesAsync();

            var modal = new
            {
                type = "modal",
                // notify_on_close = true, Will optionally inform app that modal was closed.
                callback_id = $"send-grats-modal|{draft.Id}", // view_id is also sent ad OK-response, so should probably use this instead.
                title = new
                {
                    type = "plain_text",
                    text = "Send Grats to Jonas",
                    emoji = true,
                },
                submit = new
                {
                    type = "plain_text",
                    text = "Send Grats",
                    emoji = true,
                },
                close = new
                {
                    type = "plain_text",
                    text = "Cancel",
                    emoji = true,
                },
                blocks = new object[]
                {
                    new
                    {
                        type = "input",
                        block_id = "select_user",
                        element = new
                        {
                            type = "users_select",
                            action_id = "user_selected",
                            placeholder = new
                            {
                                type = "plain_text",
                                text = "Select a user",
                                emoji = true,
                            },
                        },
                        label = new
                        {
                            type = "plain_text",
                            text = "Who should receive Grats?",
                            emoji = true,
                        },
                    },
                    new
                    {
                        type = "input",
                        block_id = "grats_message",
                        element = new
                        {
                            type = "plain_text_input",
                            action_id = "grats_message_written",
                            multiline = true,
                            placeholder = new
                            {
                                type = "plain_text",
                                text = "A short and concrete description",
                                emoji = true,
                            },
                        },
                        label = new
                        {
                            type = "plain_text",
                            text = "Why should they receive Grats?",
                            emoji = true,
                        },
                    },
                },
            };

            var response = await _slackService.OpenModal(slashCommand.TriggerId, modal);
            _telemetry.TrackEvent("Open Modal Response", new Dictionary<string, string>()
            {
                { "TriggerId", slashCommand.TriggerId },
                { "Response", response },
            });

            return Ok();
        }
    }
}
