using System.Collections.Generic;
using System.Text;
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
        public IActionResult ReceiveInteraction([FromForm(Name="payload")] string payload)
        {
            var json = HttpUtility.UrlDecode(payload, Encoding.UTF8);
            var interaction = JsonConvert.DeserializeObject<InteractionPayload>(json);

            _telemetry.TrackEvent("Received Interaction", new Dictionary<string, string>()
            {
                { "UserName", interaction.User.Name },
                { "ResponseUrl", interaction.ResponseUrl },
                { "Payload", payload },
            });

            // https://api.slack.com/reference/messaging/payload
            _slackService.ReplyToInteraction(interaction.ResponseUrl, new { text = "Grats sent!" });

            return Ok(interaction.User.Username);
        }
    }
}