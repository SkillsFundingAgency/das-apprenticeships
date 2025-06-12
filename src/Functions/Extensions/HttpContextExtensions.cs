using Microsoft.AspNetCore.Http;

namespace SFA.DAS.Learning.Functions.Extensions;

internal static class HttpContextExtensions
{
    /// <summary>
    /// WARNING. This method is a temporary measure to allow the back office to bypass claims validation while we do not have
    /// a backoffice UI in place. This method should be removed once the backoffice UI is in place.
    /// </summary>
    /// <param name="context"></param>
    internal static void MarkAsBackOfficeRequest(this HttpContext context)
    {
        context.Items["IsClaimsValidationRequired"] = false;
    }
}
