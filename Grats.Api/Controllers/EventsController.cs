using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gratify.Grats.Api.Database;
using Gratify.Grats.Api.Dto;
using Gratify.Grats.Api.Services;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

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
                .Select(user => new
                {
                    type = "section",
                    text = new
                    {
                        type = "mrkdwn",
                        text = $"*<@{user.Id}>*"
                    },
                    accessory = new
                    {
                        type = "button",
                        style = "danger",
                        text = new
                        {
                            type = "plain_text",
                            text = "Remove",
                            emoji = true
                        },
                        value = $"remove_team_member|{user.Id}"
                    }
                })
                .ToList();

            var homeBlocks = new
            {
                type = "home",
                blocks = new List<object>
                {
                    new
                    {
                        type = "section",
                        text = new
                        {
                            type = "mrkdwn",
                            text = ":rocket: *Your team*"
                        },
                        accessory = new
                        {
                            type = "button",
                            text = new
                            {
                                type = "plain_text",
                                text = ":heavy_plus_sign: New member",
                                emoji = true
                            },
                            value = "add_new_team_member"
                        }
                    },
                    new
                    {
                        type = "divider"
                    },
                }
            };

            homeBlocks.blocks.AddRange(teamMembers);
            return homeBlocks;
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveEvent([FromBody] SlackEvent slackEvent)
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

        public async Task ShowAppHome(SlackEvent slackEvent)
        {
            var userId = slackEvent.Event.User;
            var teamMembers = _database.Users.Where(user => user.GratsApprover == userId);
            var homeBlocks = AppHomeBlocks(teamMembers);
            await _slackService.PublishModal(slackEvent.Event.User, homeBlocks);
        }
    }
}
