using Microsoft.AspNetCore.Mvc;

namespace Gratify.Grats.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PingController : ControllerBase
    {
        [HttpGet]
        public string Get() => "PONG";
    }
}
