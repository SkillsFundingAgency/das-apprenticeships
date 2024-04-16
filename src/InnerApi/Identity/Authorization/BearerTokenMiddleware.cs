﻿using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace SFA.DAS.Apprenticeships.InnerApi.Identity.Authorization;

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
        if (DisableAccountAuthorization())
        {
            _logger.LogInformation("Account-level authorization has been disabled.");
            await _next(context);
            return;
        }

        RequireAccountIdClaimsValidation(context);
            
        var token = ReadTokenFromRequestHeader(context);
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Bearer token is null or empty.");
            await Write401Response(context, "Bearer token not present.");
            return;
        }

        var claims = GetClaimsFromToken(token);
        _logger.LogInformation("Claims:... {p1}", string.Join(", ", claims.Select(x => x.ToString())));
        if (!HandleProviderAccountClaim(context, claims) && !HandleEmployerAccountClaim(context, claims))
        {
            _logger.LogError("Invalid bearer token. An account id claim was not found for a provider or employer in the token.");
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
        const string ukprnClaimName = "http://schemas.portal.com/ukprn";
        var ukprns = claims.Where(x => x.Type == ukprnClaimName).Select(x => x.Value).ToArray();
        if (!ukprns.Any())
        {
            return false;
        }
        _logger.LogInformation("Ukprn claim found. {p1}", ukprn);
        _logger.LogInformation("Ukprn claim stored in HttpContext. Value stored: {p1}", context.Items["Ukprn"]);
        var accountIds = string.Join(";", ukprns);
        context.Items["Ukprn"] = accountIds;
        return true;
    }

    private bool HandleEmployerAccountClaim(HttpContext context, IEnumerable<Claim> claims)
    {
        const string employerAccountIdClaimName = "http://das/employer/identity/claims/account";
        var employerAccountIds = claims.Where(x => x.Type == employerAccountIdClaimName).Select(x => x.Value).ToArray();
        if (!employerAccountIds.Any())
        {
            return false;
        }
        _logger.LogInformation("EmployerAccountId claim found. {p1}", employerAccountId);
        _logger.LogInformation("EmployerAccountId claim stored in HttpContext. Value stored: {p1}", context.Items["EmployerAccountId"]);

        var accountIds = string.Join(";", employerAccountIds);
        context.Items["EmployerAccountId"] = accountIds;
        return true;
    }
}