using System.Security.Claims;

namespace SFA.DAS.Learning.InnerApi.Identity.Authorization;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
[Flags]
public enum UserType : byte
{
    None = 0,
    Provider = 1,
    Employer = 2,
    ServiceAccount = 4
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

internal static class UserTypeExtensions
{
    public static UserType GetUserType(this ClaimsPrincipal user)
    {
        if (user.Identity?.IsAuthenticated != true)
        {
            return UserType.None;
        }

        var ukprns = user.Claims.Where(x => x.Type == SFAClaimTypes.UkprnClaimName).Select(x => x.Value).ToArray();
        if (ukprns.Any())
        {
            return UserType.Provider;
        }

        var employerAccountIds = user.Claims.Where(x => x.Type == SFAClaimTypes.EmployerAccountIdClaimName).Select(x => x.Value).ToArray();
        if (employerAccountIds.Any())
        {
            return UserType.Employer;
        }

        var serviceAccount = user.Claims.Where(x => x.Type == SFAClaimTypes.ServiceAccountClaimName).FirstOrDefault();
        if (serviceAccount != null)
        {
            return UserType.ServiceAccount;
        }

        return UserType.None;
    }
}