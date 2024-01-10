using Microsoft.AspNetCore.Authorization;
using System.Diagnostics.CodeAnalysis;


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace SFA.DAS.Apprenticeships.InnerApi.Identity.Authorization;

[ExcludeFromCodeCoverage]
public static class AuthorizationExtensions
{
	public static IServiceCollection AddApiAuthorization(this IServiceCollection services, bool isDevelopment = false)
	{

		services.AddAuthorization(x =>
		{
			{
				x.AddPolicy("default", policy =>
				{
					if (isDevelopment)
						policy.AllowAnonymousUser();
					else
						policy.RequireAuthenticatedUser();

				});


				x.DefaultPolicy = x.GetPolicy("default")!;
			}
		});
		if (isDevelopment)
			services.AddSingleton<IAuthorizationHandler, LocalAuthorizationHandler>();
		return services;
	}
}
