using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TaskAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [AllowAnonymous]
    public class StatusController : ControllerBase
    {
        [HttpGet]
        public IActionResult Test()
        {
            return Ok("ok");
        }
    }
}
