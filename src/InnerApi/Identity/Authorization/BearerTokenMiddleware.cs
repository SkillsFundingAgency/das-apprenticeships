﻿using System.IdentityModel.Tokens.Jwt;

namespace SFA.DAS.Apprenticeships.InnerApi.Identity.Authorization
{
    /// <summary>
    /// Middleware that handles the claim values from bearer tokens in incoming requests
    /// </summary>
    public class BearerTokenMiddleware
    {
        private readonly RequestDelegate _next;

        public BearerTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            EnsureClaimsAreValidated(context);
            
            var token = ReadTokenFromAuthHeader(context);
            
            var providerAccountClaimHandled = HandleProviderAccountClaim(context, token);
            if (providerAccountClaimHandled)
            {
                await _next(context);
                return;
            }
            var employerAccountClaimHandled = HandleEmployerAccountClaim(context, token);
            if (employerAccountClaimHandled)
            {
                await _next(context);
                return;
            }
            
            throw new UnauthorizedAccessException();
        }

        private static void EnsureClaimsAreValidated(HttpContext context)
        {
            context.Items["IsClaimsValidationRequired"] = true;
        }

        private static JwtSecurityToken ReadTokenFromAuthHeader(HttpContext context)
        {
            var bearerToken = context.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(bearerToken))
            {
                throw new UnauthorizedAccessException();
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(bearerToken);
            return token;
        }

        private static bool HandleProviderAccountClaim(HttpContext context, JwtSecurityToken token)
        {
            var ukprnClaimName = "http://schemas.portal.com/ukprn";
            var ukprn = token.Claims.FirstOrDefault(c => c.Type == ukprnClaimName)?.Value;
            if (!string.IsNullOrEmpty(ukprn))
            {
                context.Items["Ukprn"] = ukprn;
                return true;
            }
            return false;
        }

        private static bool HandleEmployerAccountClaim(HttpContext context, JwtSecurityToken token)
        {
            var employerAccountIdClaimName = "http://das/employer/identity/claims/account";
            var employerAccountId = token.Claims.FirstOrDefault(c => c.Type == employerAccountIdClaimName)?.Value;
            if (!string.IsNullOrEmpty(employerAccountId))
            {
                context.Items["EmployerAccountId"] = employerAccountId;
                return true;
            }
            return false;
        }
    }
}