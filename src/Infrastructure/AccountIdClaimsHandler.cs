using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Encoding;

namespace SFA.DAS.Apprenticeships.Infrastructure;

public class AccountIdClaimsHandler : IAccountIdClaimsHandler
{
    private readonly ILogger<AccountIdClaimsHandler> _logger;
    private readonly HttpContext _httpContext;
    private readonly IEncodingService _encodingService;

    public AccountIdClaimsHandler(IHttpContextAccessor httpContextAccessor, ILogger<AccountIdClaimsHandler> _logger, IEncodingService encodingService)
    {
        this._logger = _logger;
        _encodingService = encodingService;
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

        var validationRequired = IsValidationRequired();
        _logger.LogInformation("Account ID validation flag found in HttpContext: {p1}", validationRequired.ToString());
        accountIdClaims.IsClaimsValidationRequired = validationRequired;

        if (!TryGetAccountIds(out var accountIds, out var accountType))
        {
            _logger.LogWarning("Unexpected error. No valid account id found for either Ukprn or EmployerAccountId.");
            return accountIdClaims;
        }

        accountIdClaims.AccountIds = accountIds;
        accountIdClaims.AccountIdClaimsType = accountType;
        return accountIdClaims;
    }

    private bool TryGetAccountIds(out List<long> accountIds, out AccountIdClaimsType accountType)
    {
        accountIds = new List<long>();
        accountType = default;

        if (TryGetClaims(_httpContext.Items, "Ukprn", out var ukprnValues, out accountType))
            return ParseClaims(ukprnValues, AccountIdClaimsType.Provider, accountIds);

        if (TryGetClaims(_httpContext.Items, "EmployerAccountId", out var employerAccountIdValues, out accountType) && !string.IsNullOrEmpty(employerAccountIdValues))
        {
            foreach (var claimValue in employerAccountIdValues.Split(";"))
            {
                if (!_encodingService.TryDecode(claimValue, EncodingType.AccountId, out var decodedValue))
                {
                    _logger.LogWarning("Employer account id claim value ({0}) could not be decoded.", claimValue);
                    continue;
                }

                accountIds.Add(decodedValue);
            }

            if (accountIds.Any())
            {
                return true;
            }
        }

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
            _logger.LogInformation("{0} claim value ({1}) parsed successfully", type, accountId.ToString());
        }

        return true;
    }

    private bool IsValidationRequired()
    {
        if (_httpContext.Items.TryGetValue("IsClaimsValidationRequired", out var validationRequiredValue) && validationRequiredValue is bool)
        {
            return (bool)validationRequiredValue;
        }

        _logger.LogInformation("Value for IsClaimsValidationRequired was not found in HttpContext.Items");
        return false;
    }
}