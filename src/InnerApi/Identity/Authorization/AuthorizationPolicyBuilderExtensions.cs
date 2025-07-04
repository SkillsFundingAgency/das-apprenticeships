﻿using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace SFA.DAS.Learning.InnerApi.Identity.Authorization;

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
        var possibleCombinationsOfUserTypes = GetPossibleAuthCombinations();

        foreach (var userType in possibleCombinationsOfUserTypes)
        {
            options.AddPolicy(AuthorizeUserTypeAttribute.GetPolicyName(userType, UserTypeRequirement.AuthorizeMode.ControllerLevelRequirement), policy => policy.Requirements.Add(new UserTypeRequirement(userType, UserTypeRequirement.AuthorizeMode.ControllerLevelRequirement)));
            options.AddPolicy(AuthorizeUserTypeAttribute.GetPolicyName(userType, UserTypeRequirement.AuthorizeMode.ActionLevelOverrideRequirement), policy => policy.Requirements.Add(new UserTypeRequirement(userType, UserTypeRequirement.AuthorizeMode.ActionLevelOverrideRequirement)));
        }

        return options;
    }

    public static AuthorizationOptions AddAnonymousUserTypeAuthorization(this AuthorizationOptions options)
    {
        var possibleCombinationsOfUserTypes = GetPossibleAuthCombinations();

        foreach (var userType in possibleCombinationsOfUserTypes)
        {
            options.AddPolicy(AuthorizeUserTypeAttribute.GetPolicyName(userType, UserTypeRequirement.AuthorizeMode.ControllerLevelRequirement), policy => policy.Requirements.Add(new NoneRequirement()));
            options.AddPolicy(AuthorizeUserTypeAttribute.GetPolicyName(userType, UserTypeRequirement.AuthorizeMode.ActionLevelOverrideRequirement), policy => policy.Requirements.Add(new NoneRequirement()));
        }

        return options;
    }

    /// <summary>
    /// Attributes can be applied to controllers and endpoints, some endpoints may allow only one type of user, others may allow multiple types of users.
    /// For each possible combination of user types, a policy needs to be created. This allows the attribute to be applied to the controller or endpoint
    /// with whichever combination of user types is required.
    /// </summary>
    private static List<UserType> GetPossibleAuthCombinations()
    {
        var possibleCombinationsOfUserTypes = new List<UserType>();
        possibleCombinationsOfUserTypes.Add(UserType.Provider);
        possibleCombinationsOfUserTypes.Add(UserType.Employer);
        possibleCombinationsOfUserTypes.Add(UserType.ServiceAccount);
        possibleCombinationsOfUserTypes.Add(UserType.Provider | UserType.Employer);
        possibleCombinationsOfUserTypes.Add(UserType.Provider | UserType.Employer | UserType.ServiceAccount);
        possibleCombinationsOfUserTypes.Add(UserType.Employer | UserType.ServiceAccount);
        possibleCombinationsOfUserTypes.Add(UserType.Provider | UserType.ServiceAccount);

        return possibleCombinationsOfUserTypes;
    }
}

[ExcludeFromCodeCoverage]
public class NoneRequirement : IAuthorizationRequirement
{
}