using Microsoft.AspNetCore.Authorization;
using System.Diagnostics.CodeAnalysis;
using static SFA.DAS.Apprenticeships.InnerApi.Identity.Authorization.UserTypeRequirement;

namespace SFA.DAS.Apprenticeships.InnerApi.Identity.Authorization;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

/// <summary>
/// Ensures that the user is of the correct type to access the resource
/// </summary>
/// <example>
/// If a controller or endpoint has the following attribute: [AuthorizeUserType(UserType.Provider | UserType.Employer)]
/// then only users with the UserType.Provider or UserType.Employer flag set will be able to access the resource
/// </example>
public class UserTypeAuthorizationHandler : AuthorizationHandler<UserTypeRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserTypeRequirement requirement)
    {
        if(IsOverriden(context, requirement))
        {
            context.Succeed(requirement);
            return Task.CompletedTask; // This policy is overriden by another policy, therefore do not process this policy
        }

        var userType = context.User.GetUserType();
        if(requirement.AuthorizedUserType.HasFlag(userType))
        {
            context.Succeed(requirement);  // Grant access
        }
        else
        {
            context.Fail();  // Deny access
        }

        return Task.CompletedTask;
    }

    private bool IsOverriden(AuthorizationHandlerContext context, UserTypeRequirement requirement)
    {
        var userRequirements = context.Requirements.OfType<UserTypeRequirement>();

        if (!userRequirements.Any(x=>x.Mode == AuthorizeMode.Override))
        {
            return false; // No override policies, therefore this invocation is not overriden
        }

        if(requirement.Mode == AuthorizeMode.Override)
        {
            return false; // This policy is an override policy, therefore it is not overriden and overrides all other policies
        }

        return true; // This policy is not an override policy and should be overriden by an override policy
    }
}

[ExcludeFromCodeCoverage]
public class UserTypeRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// Allows customization at endpoint level to determine whether to add or override the authorization policy
    /// If override is selected, the endpoint will only be accessible to the specified user type(s) and previously defined UserType policies will be ignored
    /// </summary>
    public enum AuthorizeMode
    {
        Add,
        Override
    }

    /// <summary>
    /// Determines the type of user that is authorized to access the resource
    /// </summary>
    public UserType AuthorizedUserType { get; private set; }

    /// <summary>
    /// Allows customization at endpoint level to determine whether to add or override the authorization policy
    /// If override is selected, the endpoint will only be accessible to the specified user type(s) and previously defined UserType policies will be ignored
    /// </summary>
    public AuthorizeMode Mode { get; private set; }

    public UserTypeRequirement(UserType userType, AuthorizeMode authorizeMode)
    {
        AuthorizedUserType = userType;
        Mode = authorizeMode;
    }
}

public class AuthorizeUserTypeAttribute : AuthorizeAttribute
{
    internal static string GetPolicyName(UserType userTypes, AuthorizeMode authorizeMode)
    {
        return $"UserType-{authorizeMode}-{(byte)userTypes}";
    }

    public AuthorizeUserTypeAttribute(UserType userTypes, AuthorizeMode authorizeMode = AuthorizeMode.Add)
    {
        Policy = GetPolicyName(userTypes, authorizeMode);
    }
}