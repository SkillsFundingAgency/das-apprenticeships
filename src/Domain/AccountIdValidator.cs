using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.Infrastructure;

namespace SFA.DAS.Apprenticeships.Domain
{
    public class AccountIdValidator : IAccountIdValidator
    {
        private readonly IAccountIdClaimsHandler _accountIdClaimsHandler;

        public AccountIdValidator(IAccountIdClaimsHandler accountIdClaimsHandler)
        {
            _accountIdClaimsHandler = accountIdClaimsHandler;
        }

        public void ValidateAccountId(ApprenticeshipDomainModel apprenticeship)
        {
            var accountIdClaims = _accountIdClaimsHandler.GetAccountIdClaims();
            switch (accountIdClaims.AccountIdClaimsType)
            {
                case AccountIdClaimsType.Provider when
                    apprenticeship.Ukprn != accountIdClaims.AccountId:
                    throw new UnauthorizedAccessException(
                        InvalidAccountIdErrorMessage(nameof(apprenticeship.Ukprn),apprenticeship.Ukprn, accountIdClaims.AccountId));
                case AccountIdClaimsType.Employer when
                    apprenticeship.EmployerAccountId != accountIdClaims.AccountId:
                    throw new UnauthorizedAccessException(
                        InvalidAccountIdErrorMessage(nameof(apprenticeship.EmployerAccountId),apprenticeship.EmployerAccountId, accountIdClaims.AccountId));
            }
        }

        private static string InvalidAccountIdErrorMessage(string accountIdName, long accountIdRequestedValue, long? accountIdClaimValue)
        {
            return $"The account id ({accountIdName}) in the requested record ({accountIdRequestedValue}) does not match the account id in the claim ({accountIdClaimValue}).";
        }
    }
}
