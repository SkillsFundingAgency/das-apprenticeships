using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

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
            _logger.LogInformation("Read token from auth header: {p1}", token);
            var principal = ValidateToken(token);
            _logger.LogInformation("Token validated.");


            if (!HandleProviderAccountClaim(context, principal) && !HandleEmployerAccountClaim(context, principal))
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

        private static string ReadTokenFromAuthHeader(HttpContext context)
        {
            var bearerToken = context.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", string.Empty);;
            if (string.IsNullOrEmpty(bearerToken))
            {
                throw new UnauthorizedAccessException();
            }
            return bearerToken;
        }

        private ClaimsPrincipal ValidateToken(string token)
        {
            var signingKey = _configuration["UserBearerTokenSigningKey"];
            if (string.IsNullOrEmpty(signingKey))
            {
                throw new ArgumentNullException($"{nameof(signingKey)}", "Signing key must be set before a token can be retrieved. This should ideally be done in startup");
            }

            var validationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(signingKey))
            };

            return new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out _);
        }

        private bool HandleProviderAccountClaim(HttpContext context, ClaimsPrincipal claimsPrincipal)
        {
            var ukprnClaimName = "http://schemas.portal.com/ukprn";
            var ukprn = claimsPrincipal.FindFirst(ukprnClaimName)?.Value;
            if (string.IsNullOrEmpty(ukprn))
            {
                return false;
            }
            context.Items["Ukprn"] = ukprn;
            _logger.LogInformation("Ukprn claim found and stored in HttpContext");
            return true;
        }

        private bool HandleEmployerAccountClaim(HttpContext context, ClaimsPrincipal claimsPrincipal)
        {
            var employerAccountIdClaimName = "http://das/employer/identity/claims/account";
            var employerAccountId = claimsPrincipal.FindFirst(employerAccountIdClaimName)?.Value;
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