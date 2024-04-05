using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.Infrastructure;

namespace SFA.DAS.Apprenticeships.DataAccess
{
    public class AccountIdAuthorizer : IAccountIdAuthorizer
    {
        private readonly ILogger<AccountIdAuthorizer> _logger;
        private readonly AccountIdClaims _accountIdClaims;

        public AccountIdAuthorizer(IAccountIdClaimsHandler accountIdClaimsHandler, ILogger<AccountIdAuthorizer> logger)
        {
            _logger = logger;
            _accountIdClaims = accountIdClaimsHandler.GetAccountIdClaims();
        }
        
        public void AuthorizeAccountId(Apprenticeship apprenticeship)
        {
            if (!_accountIdClaims.IsClaimsValidationRequired)
            {
                return;
            }

            switch (_accountIdClaims.AccountIdClaimsType)
            {
                case AccountIdClaimsType.Provider:
                    if (apprenticeship.Ukprn != _accountIdClaims.AccountId)
                    {
                        throw new UnauthorizedAccessException(
                            InvalidAccountIdErrorMessage(nameof(apprenticeship.Ukprn),apprenticeship.Ukprn, _accountIdClaims.AccountId));
                    }
                    break;
                case AccountIdClaimsType.Employer:
                    if (apprenticeship.EmployerAccountId != _accountIdClaims.AccountId)
                    {
                        throw new UnauthorizedAccessException(
                            InvalidAccountIdErrorMessage(nameof(apprenticeship.EmployerAccountId),apprenticeship.EmployerAccountId, _accountIdClaims.AccountId));
                    }
                    break;
                case null:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(InvalidAccountIdClaimsTypeErrorMessage());
            }
        }
            
        public IQueryable<Apprenticeship> ApplyAuthorizationFilterOnQueries(DbSet<Apprenticeship> apprenticeships)
        {
            if (!apprenticeships.Any())
            {
                return apprenticeships;
            }
            if (!_accountIdClaims.IsClaimsValidationRequired)
            {
                return apprenticeships;
            }

            switch (_accountIdClaims.AccountIdClaimsType)
            {
                case AccountIdClaimsType.Provider:
                    return apprenticeships.Where(x => x.Ukprn == _accountIdClaims.AccountId);
                case AccountIdClaimsType.Employer:
                    return apprenticeships.Where(x => x.EmployerAccountId == _accountIdClaims.AccountId);
                default:
                    throw new ArgumentOutOfRangeException(InvalidAccountIdClaimsTypeErrorMessage());
            }
        }

        private string InvalidAccountIdClaimsTypeErrorMessage()
        {
            return $"The {nameof(_accountIdClaims.AccountIdClaimsType)} found ('{_accountIdClaims.AccountIdClaimsType}') is not in a valid range (Provider or Employer)";
        }

        private static string InvalidAccountIdErrorMessage(string accountIdName, long accountIdRequestedValue, long? accountIdClaimValue)
        {
            return $"The account id ({accountIdName}) in the requested record ({accountIdRequestedValue}) does not match the account id in the claim ({accountIdClaimValue}).";
        }
    }
}