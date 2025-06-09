using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace SFA.DAS.Learning.InnerApi.Identity.Authorization;

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
