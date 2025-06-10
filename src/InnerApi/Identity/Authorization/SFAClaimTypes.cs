namespace SFA.DAS.Learning.InnerApi.Identity.Authorization;

/// <summary>
/// Claim type keys found in the JWT token
/// </summary>
public static class SFAClaimTypes
{
    /// <summary>
    /// UKPRN claim name, found in provider tokens
    /// </summary>
    public const string UkprnClaimName = "http://schemas.portal.com/ukprn";

    /// <summary>
    /// Employer account ID claim name, found in employer tokens
    /// </summary>
    public const string EmployerAccountIdClaimName = "http://das/employer/identity/claims/account";

    /// <summary>
    /// User ID claim name, found in employer tokens
    /// </summary>
    public const string EmployerUserIdClaimName = "http://das/employer/identity/claims/id";

    /// <summary>
    /// Service account claim name, found in service account tokens
    /// </summary>
    public const string ServiceAccountClaimName = "serviceAccount";
}
