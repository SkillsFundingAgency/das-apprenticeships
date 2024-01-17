using Microsoft.AspNetCore.Authorization;
using System.Diagnostics.CodeAnalysis;


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace SFA.DAS.Apprenticeships.InnerApi.Identity.Authorization;

[ExcludeFromCodeCoverage]
public static class AuthorizationPolicyBuilderExtensions
{
	public static AuthorizationPolicyBuilder AllowAnonymousUser(this AuthorizationPolicyBuilder builder)
	{
		builder.Requirements.Add(new NoneRequirement());
		return builder;
	}
}

[ExcludeFromCodeCoverage]
public class NoneRequirement : IAuthorizationRequirement
{
}
