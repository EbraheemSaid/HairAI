using Microsoft.AspNetCore.Mvc;

namespace HairAI.Api.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class HealthController : BaseController
{
    [HttpGet]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }
}