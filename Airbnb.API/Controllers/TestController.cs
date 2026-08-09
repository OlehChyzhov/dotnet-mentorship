using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Airbnb.API.Controllers;

[Authorize(Roles = "Host")]
[ApiController]
[Route("test")]
public class TestController : ControllerBase
{
    [HttpGet("method")]
    public IActionResult Method()
    {
        return Ok("Endpoint hit");
    }
}