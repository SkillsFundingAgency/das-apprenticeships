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

            RequireClaimsValidation(context);
            var token = ReadTokenFromAuthHeader(context);
            _logger.LogInformation("Token retrieved from auth header: {p1}", token);
            var claims = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims;
            _logger.LogInformation("Claims.");

            if (!HandleProviderAccountClaim(context, claims) && !HandleEmployerAccountClaim(context, claims))
            {
                _logger.LogInformation("Account id claim not found in bearer token.");
                throw new UnauthorizedAccessException();
            }

            await _next(context);
        }

        private bool DisableAccountAuthorization()
        {
            return bool.TryParse(_configuration["DisableAccountAuthorization"], out var disableAccountAuthorization) && disableAccountAuthorization;
        }

        private static void RequireClaimsValidation(HttpContext context)
        {
            context.Items["IsClaimsValidationRequired"] = true;
        }

        private string ReadTokenFromAuthHeader(HttpContext context)
        {
            var bearerToken = context.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", string.Empty);
            if (string.IsNullOrEmpty(bearerToken))
            {
                _logger.LogInformation("Bearer token is null or empty.");
                throw new UnauthorizedAccessException();
            }
            return bearerToken;
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