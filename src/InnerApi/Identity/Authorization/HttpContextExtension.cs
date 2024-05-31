namespace SFA.DAS.Apprenticeships.InnerApi.Identity.Authorization;

/// <summary>
/// Authorization extension for HttpContext
/// </summary>
public static class HttpContextExtension
{
    /// <summary>
    /// Get the user id from the HttpContext
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns>User Id</returns>
    /// <exception cref="UnauthorizedAccessException">Throws exception if userId not found</exception>
    public static string GetUserId(this HttpContext httpContext)
    {
        var userId = httpContext.Items["UserId"];
        if(userId == null)
        {
            throw new UnauthorizedAccessException("User Id not found in HttpContext");
        }

        return (string)userId;
    }
}
