using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace SFA.DAS.Learning.InnerApi.Identity.Authorization;

/// <summary>
/// Middleware that handles the claim values from bearer tokens in incoming API requests
/// </summary>
public class BearerTokenMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly ILogger<BearerTokenMiddleware> _logger;

    public BearerTokenMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<BearerTokenMiddleware> logger)
    {
        _next = next;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Handles the claim values from bearer tokens in incoming API requests
    /// </summary>
    public async Task Invoke(HttpContext context)
    {
        if (DoNotCheckAuthorizationForThisEndpoint(context))
        {
            await _next(context);
            return;
        }

        if (DisableAccountAuthorization())
        {
            _logger.LogInformation("Account-level authorization has been disabled.");
            await _next(context);
            return;
        }
            
        var token = ReadTokenFromRequestHeader(context);
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Bearer token is null or empty.");
            await Write401Response(context, "Bearer token not present.");
            return;
        }

        var claims = GetClaimsFromToken(token);
        if (!HandleProviderAccountClaim(context, claims) && !HandleEmployerAccountClaim(context, claims) && !HandleServiceAccountClaim(context, claims))
        {
            _logger.LogError("Invalid bearer token. Token does not have claims that match, provider, employer or serviceAccount tokens");
            await Write401Response(context, "Invalid bearer token.");
            return;
        }

        await _next(context);
    }

    private static IEnumerable<Claim> GetClaimsFromToken(string token)
    {
        var claims = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims;
        return claims;
    }

    private static string? ReadTokenFromRequestHeader(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", string.Empty);
        return token;
    }

    private static async Task Write401Response(HttpContext context, string errorMessage)
    {
        var response = context.Response;
        response.ContentType = "application/json";
        response.StatusCode = (int)HttpStatusCode.Unauthorized;
        if(response.Headers != null)
        {
            response.Headers.Append("WWW-Authenticate", errorMessage);
        }
        
        await response.WriteAsync(errorMessage);
    }

    private bool DisableAccountAuthorization()
    {
        return bool.TryParse(_configuration["DisableAccountAuthorization"], out var disableAccountAuthorization) && disableAccountAuthorization;
    }

    private static void RequireAccountIdClaimsValidation(HttpContext context)
    {
        context.Items["IsClaimsValidationRequired"] = true;
    }

    private bool HandleProviderAccountClaim(HttpContext context, IEnumerable<Claim> claims)
    {
        var ukprns = claims.Where(x => x.Type == SFAClaimTypes.UkprnClaimName).Select(x => x.Value).ToArray();
        if (!ukprns.Any())
        {
            return false;
        }
        var accountIds = string.Join(";", ukprns);
        context.Items["Ukprn"] = accountIds;
        context.Items["UserId"] = claims.Single(x => x.Type == ClaimTypes.Name).Value;

        RequireAccountIdClaimsValidation(context);

        return true;
    }

    private bool HandleEmployerAccountClaim(HttpContext context, IEnumerable<Claim> claims)
    {

        var employerAccountIds = claims.Where(x => x.Type == SFAClaimTypes.EmployerAccountIdClaimName).Select(x => x.Value).ToArray();
        if (!employerAccountIds.Any())
        {
            return false;
        }

        var accountIds = string.Join(";", employerAccountIds);
        context.Items["EmployerAccountId"] = accountIds;
        context.Items["UserId"] = claims.Single(x => x.Type == SFAClaimTypes.EmployerUserIdClaimName).Value;

        RequireAccountIdClaimsValidation(context);

        return true;
    }

    private bool HandleServiceAccountClaim(HttpContext context, IEnumerable<Claim> claims)
    {
        var serviceAccount = claims.Where(x => x.Type == SFAClaimTypes.ServiceAccountClaimName).FirstOrDefault();
        if (serviceAccount == null)
        {
            return false;
        }


        context.Items["ServiceAccount"] = serviceAccount.Value;
        context.Items["sub"] = claims.Single(x => x.Type == "sub").Value;
        return true;
    }

    private bool DoNotCheckAuthorizationForThisEndpoint(HttpContext context)
    {
        var endpoint = context.GetEndpoint();

        if(endpoint == null)
            return false;

        if (endpoint.Metadata.GetMetadata<AllowAnonymousAttribute>() != null)
            return true;

        if(endpoint.DisplayName == "Health checks")
            return true;

        return false;
    }
}