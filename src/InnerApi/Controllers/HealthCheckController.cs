using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.Apprenticeships.InnerApi.Controllers;

[ExcludeFromCodeCoverage]
[Route("")]
[ApiController]
[AllowAnonymous]
public class HealthCheckController(IConfiguration config) : Controller
{
    [HttpGet("")]
    [ProducesResponseType(typeof(string), 200)]
    public IActionResult Index()
    {
        return new OkObjectResult("Healthy");
    }

    [HttpGet("info")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetInfo()
    {
        var info = new
        {
            Version = config["Version"] ?? "1.0.0",
            Name = "Apprenticeships API"
        };
        return Ok(info);
    }
}
