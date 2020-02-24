using System.Collections.Generic;
using System.Threading.Tasks;
using Gratify.Grats.Api.Database;
using Gratify.Grats.Api.Services;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Slack.Client.BlockKit.BlockElements;
using Slack.Client.BlockKit.BlockElements.Selects;
using Slack.Client.BlockKit.CompositionObjects;
using Slack.Client.BlockKit.LayoutBlocks;

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
        public async Task<IActionResult> SendGrats([FromForm] Dto.SlashCommand slashCommand)
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
                title = new PlainText
                {
                    Text = "Send Grats to Jonas",
                    Emoji = true,
                },
                submit = new PlainText
                {
                    Text = "Send Grats",
                    Emoji = true,
                },
                close = new PlainText
                {
                    Text = "Cancel",
                    Emoji = true,
                },
                blocks = new LayoutBlock[]
                {
                    new Input
                    {
                        BlockId = "select_user",
                        Element = new UsersSelect
                        {
                            ActionId = "user_selected",
                            Placeholder = new PlainText
                            {
                                Text = "Select a user",
                                Emoji = true,
                            },
                        },
                        Label = new PlainText
                        {
                            Text = "Who should receive Grats?",
                            Emoji = true,
                        },
                    },
                    new Input
                    {
                        BlockId = "grats_message",
                        Element = new PlainTextInput
                        {
                            ActionId = "grats_message_written",
                            Multiline = true,
                            Placeholder = new PlainText
                            {
                                Text = "A short and concrete description",
                                Emoji = true,
                            },
                        },
                        Label = new PlainText
                        {
                            Text = "Why should they receive Grats?",
                            Emoji = true,
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
