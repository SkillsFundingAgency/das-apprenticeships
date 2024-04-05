using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using SFA.DAS.Apprenticeships.Infrastructure.Configuration;
using System.Diagnostics.CodeAnalysis;
using Microsoft.IdentityModel.Tokens;


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace SFA.DAS.Apprenticeships.InnerApi.Identity.Authentication;

[ExcludeFromCodeCoverage]
public static class AuthenticationExtensions
{
	public static IServiceCollection AddApiAuthentication(this IServiceCollection services, IConfiguration configuration, bool isDevelopment = false)
	{
        var signingKey = configuration["UserBearerTokenSigningKey"];
        if(signingKey == null)
        {
            throw new InvalidOperationException("UserBearerTokenSigningKey is not set in configuration");
        }

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
          .AddJwtBearer(options => {
              options.RequireHttpsMetadata = false;

              options.TokenValidationParameters = new TokenValidationParameters
              {
                  ValidateIssuerSigningKey = true,
                  IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(signingKey)),
                  ValidateLifetime = true,
                  ValidateAudience = false,
                  ValidateIssuer = false
              };
          });
        return services;
	}
}
