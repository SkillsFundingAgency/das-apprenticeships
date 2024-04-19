using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.Infrastructure;

namespace SFA.DAS.Apprenticeships.DataAccess;

public class AccountIdAuthorizer : IAccountIdAuthorizer
{
    private readonly ILogger<AccountIdAuthorizer> _logger;
    private readonly AccountIdClaims _accountIdClaims;

    public AccountIdAuthorizer(IAccountIdClaimsHandler accountIdClaimsHandler, ILogger<AccountIdAuthorizer> logger)
    {
        _logger = logger;
        _logger.LogInformation("AccountIdAuthorizer controller instantiation");
        _accountIdClaims = accountIdClaimsHandler.GetAccountIdClaims();
        _logger.LogInformation("Claims fetched in AccountIdAuthorizer:... Type: {p1}, Ids: {p2}", _accountIdClaims.AccountIdClaimsType, _accountIdClaims.AccountIds);
    }
        
    public void AuthorizeAccountId(Apprenticeship apprenticeship)
    {
        _logger.LogInformation("Starting method AuthorizeAccountId... Claims being handled at the time = Type: {p1}, Id: {p2}", _accountIdClaims.AccountIdClaimsType, _accountIdClaims.AccountIds);
        if (!_accountIdClaims.IsClaimsValidationRequired)
        {
            _logger.LogInformation("Account ID claims validation is not flagged as required.");
            return;
        }

        switch (_accountIdClaims.AccountIdClaimsType)
        {
            case AccountIdClaimsType.Provider:
                if (!_accountIdClaims.AccountIds.Any(x => x == apprenticeship.Ukprn)) {
                    throw new UnauthorizedAccessException(InvalidAccountIdErrorMessage(nameof(apprenticeship.Ukprn), apprenticeship.Ukprn));
                }
                break;
            case AccountIdClaimsType.Employer:
                if (!_accountIdClaims.AccountIds.Any(x => x == apprenticeship.EmployerAccountId)) {
                    throw new UnauthorizedAccessException(InvalidAccountIdErrorMessage(nameof(apprenticeship.EmployerAccountId),apprenticeship.EmployerAccountId));
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(InvalidAccountIdClaimsTypeErrorMessage());
        }
    }
            
    public IQueryable<Apprenticeship> ApplyAuthorizationFilterOnQueries(DbSet<Apprenticeship> apprenticeships)
    {
        _logger.LogInformation("Starting method ApplyAuthorizationFilterOnQueries... Claims being handled at the time = Type: {p1}, Ids: {p2}", _accountIdClaims.AccountIdClaimsType, _accountIdClaims.AccountIds);
        if (!apprenticeships.Any())
        {
            _logger.LogInformation("No apprenticeships Account ID claims validation is not flagged as required.");
            return apprenticeships;
        }
        if (!_accountIdClaims.IsClaimsValidationRequired)
        {
            _logger.LogInformation("Account ID claims validation is not flagged as required.");
            return apprenticeships;
        }

        switch (_accountIdClaims.AccountIdClaimsType)
        {
            case AccountIdClaimsType.Provider:
                return apprenticeships.Where(x => _accountIdClaims.AccountIds.Contains(x.Ukprn));
            case AccountIdClaimsType.Employer:
                return apprenticeships.Where(x => _accountIdClaims.AccountIds.Contains(x.EmployerAccountId));
            default:
                throw new ArgumentOutOfRangeException(InvalidAccountIdClaimsTypeErrorMessage());
        }
    }

    private string InvalidAccountIdClaimsTypeErrorMessage()
    {
        return $"The {nameof(_accountIdClaims.AccountIdClaimsType)} found ('{_accountIdClaims.AccountIdClaimsType}') is not in a valid range (Provider or Employer)";
    }

    private string InvalidAccountIdErrorMessage(string accountIdName, long accountIdRequestedValue)
    {
        var ids = _accountIdClaims.AccountIds == null ? "" : string.Join(";", _accountIdClaims.AccountIds);
        return $"The account id ({accountIdName}) in the requested record ({accountIdRequestedValue}) does not match any of the account ids in the claim ({ids}).";
    }
}