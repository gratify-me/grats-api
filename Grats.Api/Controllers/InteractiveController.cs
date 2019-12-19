using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Gratify.Grats.Api.Dto;
using Gratify.Grats.Api.Services;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// https://api.slack.com/interactivity/handling
namespace Gratify.Grats.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InteractiveController : ControllerBase
    {
        private readonly ISlackService _slackService;
        private TelemetryClient _telemetry;

        public InteractiveController(ISlackService slackService, TelemetryClient telemetry)
        {
            _slackService = slackService;
            _telemetry = telemetry;
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveInteraction([FromForm(Name="payload")] string payload)
        {
            var json = HttpUtility.UrlDecode(payload, Encoding.UTF8);
            var interaction = JsonConvert.DeserializeObject<InteractionPayload>(json);

            _telemetry.TrackEvent("Received Interaction", new Dictionary<string, string>()
            {
                { "UserName", interaction.User.Name },
                { "ResponseUrl", interaction.ResponseUrl },
                { "Payload", payload },
            });

            if (interaction.Actions.Any(action => action.Value == "send_grats"))
            {
                // https://api.slack.com/reference/messaging/payload
                await _slackService.ReplyToInteraction(interaction.ResponseUrl, new { text = "Grats sent!" });
            }
            else
            {
                await _slackService.ReplyToInteraction(interaction.ResponseUrl, new { text = "Ok! Maybe next time :-)" });
            }

            return Ok(interaction.User.Username);
        }
    }
}
