using Microsoft.AspNetCore.Authorization;
using System.Diagnostics.CodeAnalysis;


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace SFA.DAS.Apprenticeships.InnerApi.Identity.Authorization;

[ExcludeFromCodeCoverage]
public static class AuthorizationExtensions
{
	public static IServiceCollection AddApiAuthorization(this IServiceCollection services, bool isDevelopment = false)
	{

        if (isDevelopment)
        {
            services.AddAuthorization(x =>
            {
                {
                    x.AddPolicy("default", policy =>policy.AllowAnonymousUser());
                    x.AddAnonymousUserTypeAuthorization();
                    x.DefaultPolicy = x.GetPolicy("default")!;
                }
            });
            services.AddSingleton<IAuthorizationHandler, LocalAuthorizationHandler>();
        }
        else
        {
            services.AddAuthorization(x =>
            {
                {
                    x.AddPolicy("default", policy => policy.RequireAuthenticatedUser());
                    x.AddUserTypeAuthorization();
                    x.DefaultPolicy = x.GetPolicy("default")!;
                }
            });
            services.AddSingleton<IAuthorizationHandler, UserTypeAuthorizationHandler>();
        }

        return services;
	}
}
