using Microsoft.AspNetCore.Mvc;
using Slack.Client.Views;

namespace Gratify.Api.Controllers
{
    public static class ResponseActionExtension
    {
        public static ActionResult Result(this ResponseAction responseAction) =>
            responseAction switch
            {
                ResponseActionClose _ => new OkResult(),
                _ => new OkObjectResult(responseAction),
            };
    }
}
