using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace SFA.DAS.Apprenticeships.InnerApi.Identity.Authorization
{
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
                await _next(context);
                return;
            }

            RequireAccountIdClaimsValidation(context);
            
            var token = ReadTokenFromRequestHeader(context);
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogInformation("Bearer token is null or empty.");
                await Write401Response(context, "Bearer token not present.");
                return;
            }
            _logger.LogInformation("Token retrieved from auth header: {p1}", token);

            var claims = GetClaimsFromToken(token);
            if (!HandleProviderAccountClaim(context, claims) && !HandleEmployerAccountClaim(context, claims))
            {
                _logger.LogInformation("Invalid bearer token. Account id claim not found in bearer token.");
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
            var ukprnClaimName = "http://schemas.portal.com/ukprn";
            var ukprn = claims.FirstOrDefault(x => x.Type == ukprnClaimName)?.Value;
            if (string.IsNullOrEmpty(ukprn))
            {
                return false;
            }
            context.Items["Ukprn"] = ukprn;
            _logger.LogInformation("Ukprn claim found and stored in HttpContext");
            return true;
        }

        private bool HandleEmployerAccountClaim(HttpContext context, IEnumerable<Claim> claims)
        {
            var employerAccountIdClaimName = "http://das/employer/identity/claims/account";
            var employerAccountId = claims.FirstOrDefault(x => x.Type == employerAccountIdClaimName)?.Value;
            if (string.IsNullOrEmpty(employerAccountId))
            {
                return false;
            }
            context.Items["EmployerAccountId"] = employerAccountId;
            _logger.LogInformation("EmployerAccountId claim found and stored in HttpContext");
            return true;
        }
    }
}