using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gratify.Grats.Api.Database;
using Gratify.Grats.Api.Services;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Slack.Client.BlockKit.BlockElements;
using Slack.Client.BlockKit.CompositionObjects;
using Slack.Client.BlockKit.LayoutBlocks;

// https://api.slack.com/events-api
namespace Gratify.Grats.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly ISlackService _slackService;
        private readonly GratsDb _database;
        private readonly TelemetryClient _telemetry;

        public EventsController(ISlackService slackService, GratsDb database, TelemetryClient telemetry)
        {
            _slackService = slackService;
            _database = database;
            _telemetry = telemetry;
        }

        public static object AppHomeBlocks(IEnumerable<Database.User> users)
        {
            var teamMembers = users
                .Select(user => new Section
                {
                    Text = new MrkdwnText
                    {
                        Text = $"*<@{user.Id}>*"
                    },
                    Accessory = new Button
                    {
                        Style = "danger",
                        Text = new PlainText
                        {
                            Text = "Remove",
                            Emoji = true,
                        },
                        Value = $"remove_team_member|{user.Id}",
                    }
                })
                .ToList();

            var homeBlocks = new
            {
                type = "home",
                blocks = new List<LayoutBlock>
                {
                    new Section
                    {
                        Text = new MrkdwnText
                        {
                            Text = ":rocket: *Your team*",
                        },
                        Accessory = new Button
                        {
                            Text = new PlainText
                            {
                                Text = ":heavy_plus_sign: New member",
                                Emoji = true,
                            },
                            Value = "add_new_team_member",
                        }
                    },
                    new Divider(),
                }
            };

            homeBlocks.blocks.AddRange(teamMembers);
            return homeBlocks;
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveEvent([FromBody] Dto.SlackEvent slackEvent)
        {
            if (slackEvent.IsUrlVerification)
            {
                return Ok(new
                { challenge = slackEvent.Challenge });
            }
            else if (slackEvent.Event.IsAppHomeOpened)
            {
                await ShowAppHome(slackEvent);
            }

            return Ok();
        }

        public async Task ShowAppHome(Dto.SlackEvent slackEvent)
        {
            var userId = slackEvent.Event.User;
            var teamMembers = _database.Users.Where(user => user.GratsApprover == userId);
            var homeBlocks = AppHomeBlocks(teamMembers);
            await _slackService.PublishModal(slackEvent.Event.User, homeBlocks);
        }
    }
}
