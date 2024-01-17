using Microsoft.AspNetCore.Authorization;
using System.Diagnostics.CodeAnalysis;


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace SFA.DAS.Apprenticeships.InnerApi.Identity.Authorization;

[ExcludeFromCodeCoverage]
public class LocalAuthorizationHandler : AuthorizationHandler<NoneRequirement>
{
	protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
		NoneRequirement requirement)
	{
		context.Succeed(requirement);
		return Task.CompletedTask;
	}
}
