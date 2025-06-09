using SFA.DAS.Learning.Enums;

namespace SFA.DAS.Learning.Infrastructure;

public interface IAccountIdClaimsHandler
{
    AccountIdClaims GetAccountIdClaims();
}