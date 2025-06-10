using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using static SFA.DAS.Learning.InnerApi.Identity.Authorization.UserTypeRequirement;

namespace SFA.DAS.Learning.InnerApi.Identity.Authorization;
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
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserTypeAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserTypeRequirement requirement)
    {
        if(context.User.Identity?.IsAuthenticated == false)
        {
            context.Fail(); // Deny access
            _httpContextAccessor.HttpContext?.Response.Headers.Append("X-Authorization-Failure", "User not authenticated");
            return Task.CompletedTask;
        }

        if (IsOverriden(context, requirement))
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
            _httpContextAccessor.HttpContext?.Response.Headers.Append("X-Authorization-Failure", $"{userType.ToString()} cannot access requested endpoint");
        }

        return Task.CompletedTask;
    }

    private bool IsOverriden(AuthorizationHandlerContext context, UserTypeRequirement requirement)
    {
        var userRequirements = context.Requirements.OfType<UserTypeRequirement>();

        if (!userRequirements.Any(x=>x.Mode == AuthorizeMode.ActionLevelOverrideRequirement))
        {
            return false; // No override policies, therefore this invocation is not overriden
        }

        if(requirement.Mode == AuthorizeMode.ActionLevelOverrideRequirement)
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
        /// <summary>
        /// Add this requirement to the existing set of requirements for use at controller level
        /// </summary>
        ControllerLevelRequirement,
        /// <summary>
        /// Override existing requirements set at controller level and only require this one
        /// </summary>
        ActionLevelOverrideRequirement
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

public abstract class AuthorizeUserTypeAttribute : AuthorizeAttribute
{
    internal static string GetPolicyName(UserType userTypes, AuthorizeMode authorizeMode)
    {
        return $"UserType-{authorizeMode}-{(byte)userTypes}";
    }

    protected AuthorizeUserTypeAttribute(UserType userTypes, AuthorizeMode authorizeMode)
    {
        Policy = GetPolicyName(userTypes, authorizeMode);
    }
}

/// <summary>
/// Set authorization user type requirements at controller level
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ControllerAuthorizeUserTypeAttribute : AuthorizeUserTypeAttribute
{
    /// <summary>
    /// Set authorization user type requirements at controller level
    /// </summary>
    /// <param name="userTypes">The user types which are allowed to call actions on this controller</param>
    public ControllerAuthorizeUserTypeAttribute(UserType userTypes) : base(userTypes, AuthorizeMode.ControllerLevelRequirement)
    {
    }
}

/// <summary>
/// Set authorization user type requirements at action level, overrides controller level requirements
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class ActionAuthorizeUserTypeAttribute : AuthorizeUserTypeAttribute
{
    /// <summary>
    /// Set authorization user type requirements at action level, overrides controller level requirements
    /// </summary>
    /// <param name="userTypes">The user types which are allowed to call this action</param>
    public ActionAuthorizeUserTypeAttribute(UserType userTypes) : base(userTypes, AuthorizeMode.ActionLevelOverrideRequirement)
    {
    }
}