using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.Infrastructure;

public class AccountIdClaimsHandler : IAccountIdClaimsHandler
{
    private readonly ILogger<AccountIdClaimsHandler> _logger;
    private readonly HttpContext _httpContext;

    public AccountIdClaimsHandler(IHttpContextAccessor httpContextAccessor, ILogger<AccountIdClaimsHandler> _logger)
    {
        this._logger = _logger;
        _httpContext = httpContextAccessor.HttpContext;
    }

    public AccountIdClaims GetAccountIdClaims()
    {
        var accountIdClaims = new AccountIdClaims();

        _logger.LogInformation("Fetching the account ID claim values from HttpContext.");
        if (_httpContext != null && _httpContext.Items != null)
        {
            _logger.LogInformation("HttpContext.Items:... {p1}", string.Join(", ", _httpContext.Items));
            if (TryGetAccountId(_httpContext.Items, out var accountId, out var accountType))
            {
                _logger.LogInformation("Account ID claims found. {accountType}: {accountId}", accountType, accountId);
                accountIdClaims.AccountId = accountId;
                accountIdClaims.AccountIdClaimsType = accountType;
            }

            if (TryGetValidationRequired(_httpContext.Items, out var validationRequired))
            {
                _logger.LogInformation("Account ID validation flag found in HttpContext: {p1}", validationRequired.ToString());
                accountIdClaims.IsClaimsValidationRequired = validationRequired;
            }
        }
        else
        {
            _logger.LogWarning("Unexpected error. HttpContext or HttpContext.Items is null.");
        }

        return accountIdClaims;
    }

    private bool TryGetAccountId(IDictionary<object, object> httpContextItems, out long accountId, out AccountIdClaimsType accountType)
    {
        accountId = 0;
        accountType = default;

        if (httpContextItems.TryGetValue("Ukprn", out var ukprnValue) && long.TryParse(ukprnValue as string, out var ukprn))
        {
            _logger.LogInformation("Ukprn claim found in HttpContext (before trying to parse). Value: {p1}", ukprnValue.ToString());
            accountId = ukprn;
            accountType = AccountIdClaimsType.Provider;
            _logger.LogInformation("Ukprn claim found in HttpContext. Account type: {p1}, account Id: {p2}", accountType, accountId);
            return true;
        }
        
        if (httpContextItems.TryGetValue("EmployerAccountId", out var employerAccountIdValue) && long.TryParse(employerAccountIdValue as string, out var employerAccountId))
        {
            _logger.LogInformation("EmployerAccountId claim found in HttpContext (before trying to parse). Value: {p1}", employerAccountIdValue.ToString());
            accountId = employerAccountId;
            accountType = AccountIdClaimsType.Employer;
            _logger.LogInformation("EmployerAccountId claim found in HttpContext. Account type: {p1}, account Id: {p2}", accountType, accountId);
            return true;
        }

        _logger.LogWarning("Unexpected error. No account id found for either Ukprn or EmployerAccountId.");
        return false;
    }

    private static bool TryGetValidationRequired(IDictionary<object, object> httpContextItems, out bool validationRequired)
    {
        validationRequired = false;

        if (httpContextItems.TryGetValue("IsClaimsValidationRequired", out var validationRequiredValue) && validationRequiredValue is bool)
        {
            validationRequired = (bool)validationRequiredValue;
            return true;
        }

        return false;
    }
}