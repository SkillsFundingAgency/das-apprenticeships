using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Apprenticeships.InnerApi.Controllers;

[ExcludeFromCodeCoverage]
[Route("")]
[ApiController]
[AllowAnonymous]
public class HealthCheckController : Controller
{
    [HttpGet("")]
    [ProducesResponseType(typeof(string), 200)]
    public IActionResult Index()
    {
        return new OkObjectResult( "Healthy");
    }
}
