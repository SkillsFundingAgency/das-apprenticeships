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
        _logger.LogInformation("Fetching the relevant claim values from HttpContext....");
        var accountIdClaims = new AccountIdClaims();

        if (_httpContext == null || _httpContext.Items == null)
        {
            _logger.LogWarning("Unexpected error. HttpContext or HttpContext.Items is null.");
            return accountIdClaims;
        }
        _logger.LogInformation("HttpContext.Items:... {p1}", string.Join(", ", _httpContext.Items));

        TryGetValidationRequired(_httpContext.Items, out var validationRequired);
        _logger.LogInformation("Account ID validation flag found in HttpContext: {p1}", validationRequired.ToString());
        accountIdClaims.IsClaimsValidationRequired = validationRequired;

        if (!TryGetAccountIds(_httpContext.Items, out var accountIds, out var accountType))
        {
            _logger.LogWarning("Unexpected error. No account id found for either Ukprn or EmployerAccountId.");
            return accountIdClaims;
        }

        accountIdClaims.AccountIds = accountIds;
        accountIdClaims.AccountIdClaimsType = accountType;
        return accountIdClaims;
    }

    private bool TryGetAccountIds(IDictionary<object, object> httpContextItems, out List<long> accountIds, out AccountIdClaimsType accountType)
    {
        accountIds = new List<long>();
        accountType = default;

        if (TryGetClaims(httpContextItems, "Ukprn", out var ukprnValues, out accountType))
            return ParseClaims(ukprnValues, AccountIdClaimsType.Provider, accountIds);

        if (TryGetClaims(httpContextItems, "EmployerAccountId", out var employerAccountIdValues, out accountType))
            return ParseClaims(employerAccountIdValues, AccountIdClaimsType.Employer, accountIds);

        return false;
    }

    private bool TryGetClaims(IDictionary<object, object> httpContextItems, string key, out string? claim, out AccountIdClaimsType accountType)
    {
        claim = null;
        accountType = default;

        if (httpContextItems.TryGetValue(key, out var values))
        {
            claim = values.ToString();
            _logger.LogInformation("{0} claims found in HttpContext (before trying to parse). Value: {1}", key, claim);
            accountType = key == "Ukprn" ? AccountIdClaimsType.Provider : AccountIdClaimsType.Employer;
            return true;
        }

        return false;
    }

    private bool ParseClaims(string values, AccountIdClaimsType type, List<long> accountIds)
    {
        foreach (var claimValue in values.Split(";"))
        {
            if (!long.TryParse(claimValue, out var accountId))
            {
                _logger.LogWarning("{0} claim ({1}) could not be successfully parsed to long value.", type, claimValue);
                return false;
            }

            accountIds.Add(accountId);
            _logger.LogInformation("{0} claim value ({1) parsed successfully... account Ids: {1}", type, accountId.ToString());
        }

        return true;
    }

    private bool TryGetValidationRequired(IDictionary<object, object> httpContextItems, out bool validationRequired)
    {
        validationRequired = false;

        if (httpContextItems.TryGetValue("IsClaimsValidationRequired", out var validationRequiredValue) && validationRequiredValue is bool)
        {
            validationRequired = (bool)validationRequiredValue;
            return true;
        }
        _logger.LogWarning("Unexpected error. Value for IsClaimsValidationRequired was not found in HttpContext.Items");
        return false;
    }
}