using System.Text;
using System.Web;
using Gratify.Grats.Api.Dto;
using Gratify.Grats.Api.Services;
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

        public InteractiveController(ISlackService slackService)
        {
            _slackService = slackService;
        }

        [HttpPost]
        public string ReceiveInteraction([FromForm(Name="payload")] string payload)
        {
            var json = HttpUtility.UrlDecode(payload, Encoding.UTF8);
            var interaction = JsonConvert.DeserializeObject<InteractionPayload>(json);

            _slackService.ReplyToInteraction(interaction.ResponseUrl, "Grats sent!");

            return interaction.User.Username;
        }
    }
}
