using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SFA.DAS.Learning.InnerApi.Identity.Authorization;

namespace SFA.DAS.Apprenticeships.InnerApi.UnitTests;

internal class TestAuthorizationHelper
{
    internal static ClaimsPrincipal CreateClaimsPrincipleForProvider(string ukprnClaimName = "UkprnClaimName")
    {
        var claims = new List<Claim>
            {
                new Claim(SFAClaimTypes.UkprnClaimName, ukprnClaimName)
            };

        var identity = new ClaimsIdentity(claims, "TestAuthenticationType");
        var principal = new ClaimsPrincipal(identity);

        return principal;
    }

    internal static ClaimsPrincipal CreateClaimsPrincipleForEmployer(string employerAccountIdClaimName = "employerAccountIdClaimName")
    {
        var claims = new List<Claim>
            {
                new Claim(SFAClaimTypes.EmployerAccountIdClaimName, employerAccountIdClaimName)
            };

        var identity = new ClaimsIdentity(claims, "TestAuthenticationType");
        var principal = new ClaimsPrincipal(identity);

        return principal;
    }

    internal static string CreateValidToken(string signingKey, List<Claim> claims)
    {
        var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(signingKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
