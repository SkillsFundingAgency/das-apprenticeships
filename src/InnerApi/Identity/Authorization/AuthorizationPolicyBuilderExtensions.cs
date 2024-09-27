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

	public static AuthorizationOptions AddUserTypeAuthorization(this AuthorizationOptions options)
    {
        var possibleCombinationsOfUserTypes = new List<UserType>();
        possibleCombinationsOfUserTypes.Add(UserType.Provider);
        possibleCombinationsOfUserTypes.Add(UserType.Employer);
        possibleCombinationsOfUserTypes.Add(UserType.ServiceAccount);
        possibleCombinationsOfUserTypes.Add(UserType.Provider | UserType.Employer);
        possibleCombinationsOfUserTypes.Add(UserType.Provider | UserType.Employer | UserType.ServiceAccount);
        possibleCombinationsOfUserTypes.Add(UserType.Employer | UserType.ServiceAccount);
        possibleCombinationsOfUserTypes.Add(UserType.Provider | UserType.ServiceAccount);

        foreach (var userType in possibleCombinationsOfUserTypes)
        {
            options.AddPolicy(AuthorizeUserTypeAttribute.GetPolicyName(userType, UserTypeRequirement.AuthorizeMode.Add), policy => policy.Requirements.Add(new UserTypeRequirement(userType, UserTypeRequirement.AuthorizeMode.Add)));
            options.AddPolicy(AuthorizeUserTypeAttribute.GetPolicyName(userType, UserTypeRequirement.AuthorizeMode.Override), policy => policy.Requirements.Add(new UserTypeRequirement(userType, UserTypeRequirement.AuthorizeMode.Override)));
        }

        return options;
    }
}

[ExcludeFromCodeCoverage]
public class NoneRequirement : IAuthorizationRequirement
{
}