using Moq;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.Infrastructure;

namespace SFA.DAS.Apprenticeships.TestHelpers
{
    public class AuthorizationHelper
    {
        public static Mock<IAccountIdClaimsHandler> MockAccountIdClaimsHandler(long accountId, AccountIdClaimsType type)
        {
            var accountIdClaims = new AccountIdClaims()
            {
                AccountId = accountId,
                AccountIdClaimsType = type
            };
            var mockAccountIdClaimsHandler = new Mock<IAccountIdClaimsHandler>();
            mockAccountIdClaimsHandler.Setup(x => x.GetAccountIdClaims()).Returns(accountIdClaims);
            return mockAccountIdClaimsHandler;
        }
    }
}
