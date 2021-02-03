using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gratify.Api.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("[controller]")]
    public class PingController : ControllerBase
    {
        [HttpGet]
        public string Get() => "PONG";
    }
}
