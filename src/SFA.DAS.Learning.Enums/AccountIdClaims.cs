namespace SFA.DAS.Apprenticeships.Enums;

public enum AccountIdClaimsType
{
    Provider = 0,
    Employer = 1
}

public class AccountIdClaims
{
    public bool IsClaimsValidationRequired { get; set; }
    public IEnumerable<long>? AccountIds { get; set; }
    public AccountIdClaimsType? AccountIdClaimsType { get; set; }
}