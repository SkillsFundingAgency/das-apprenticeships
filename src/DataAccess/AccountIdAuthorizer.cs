﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.DataAccess.Extensions;
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
        _accountIdClaims = accountIdClaimsHandler.GetAccountIdClaims();
    }
        
    public void AuthorizeAccountId(Apprenticeship apprenticeship)
    {
        if (!_accountIdClaims.IsClaimsValidationRequired)
        {
            _logger.LogInformation("Account ID claims validation is not flagged as required.");
            return;
        }

        var currentEpisode = apprenticeship.GetCurrentEpisode();

        switch (_accountIdClaims.AccountIdClaimsType)
        {
            case AccountIdClaimsType.Provider:
                if (!_accountIdClaims.AccountIds.Any(x => x == currentEpisode.Ukprn))
                {
                    throw new UnauthorizedAccessException(InvalidAccountIdErrorMessage(nameof(currentEpisode.Ukprn), currentEpisode.Ukprn));
                }
                break;
            case AccountIdClaimsType.Employer:
                if (!_accountIdClaims.AccountIds.Any(x => x == currentEpisode.EmployerAccountId))
                {
                    throw new UnauthorizedAccessException(InvalidAccountIdErrorMessage(nameof(currentEpisode.EmployerAccountId), currentEpisode.EmployerAccountId));
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(InvalidAccountIdClaimsTypeErrorMessage());
        }
    }
            
    public IQueryable<Apprenticeship> ApplyAuthorizationFilterOnQueries(DbSet<Apprenticeship> apprenticeships)
    {
        if (!apprenticeships.Any())
        {
            _logger.LogInformation("There are no Apprenticeships in the DbSet.");
            return apprenticeships;
        }
        if (!_accountIdClaims.IsClaimsValidationRequired)
        {
            _logger.LogInformation("Account ID claims validation is not flagged as required.");
            return apprenticeships;
        }

        if (_accountIdClaims.AccountIds == null)
        {
            _logger.LogInformation("There are no account IDs in the account ID claims.");
            return apprenticeships;
        }

        var apprenticeshipsWithEpisodes = apprenticeships
            .Include(x => x.Episodes);

        switch (_accountIdClaims.AccountIdClaimsType)
        {
            case AccountIdClaimsType.Provider:
                return apprenticeshipsWithEpisodes
                    .Where(x => _accountIdClaims.AccountIds
                        .Contains(x.GetCurrentEpisode().Ukprn));
            case AccountIdClaimsType.Employer:
                return apprenticeshipsWithEpisodes
                    .Where(x => _accountIdClaims.AccountIds
                        .Contains(x.GetCurrentEpisode().EmployerAccountId));
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