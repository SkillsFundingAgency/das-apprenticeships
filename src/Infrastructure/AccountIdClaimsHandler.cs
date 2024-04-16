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

        if (_httpContext == null || _httpContext.Items == null)
        {
            return accountIdClaims;
        }

        TryGetValidationRequired(_httpContext.Items, out var validationRequired);
        accountIdClaims.IsClaimsValidationRequired = validationRequired;

        if (!TryGetAccountIds(_httpContext.Items, out var accountIds, out var accountType))
        {
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
                return false;
            }

            accountIds.Add(accountId);
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
        return false;
    }
}