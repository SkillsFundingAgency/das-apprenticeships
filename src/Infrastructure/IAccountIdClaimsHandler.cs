using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.Infrastructure;

public interface IAccountIdClaimsHandler
{
    AccountIdClaims GetAccountIdClaims();
}