using Microsoft.AspNetCore.Http;
using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.Infrastructure
{
    public class AccountIdClaimsHandler : IAccountIdClaimsHandler
    {
        private readonly HttpContext _httpContext;

        public AccountIdClaimsHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor.HttpContext;
        }

        public AccountIdClaims GetAccountIdClaims()
        {
            var accountIdClaims = new AccountIdClaims();

            if (_httpContext != null)
            {
                if (TryGetAccountId(_httpContext.Items, out var accountId, out var accountType))
                {
                    accountIdClaims.AccountId = accountId;
                    accountIdClaims.AccountIdClaimsType = accountType;
                }

                if (TryGetValidationRequired(_httpContext.Items, out var validationRequired))
                {
                    accountIdClaims.IsClaimsValidationRequired = validationRequired;
                }
            }

            return accountIdClaims;
        }

        private static bool TryGetAccountId(IDictionary<object, object> httpContextItems, out long accountId, out AccountIdClaimsType accountType)
        {
            accountId = 0;
            accountType = default;

            if (httpContextItems != null && httpContextItems.TryGetValue("Ukprn", out var ukprnValue) && long.TryParse(ukprnValue as string, out var ukprn))
            {
                accountId = ukprn;
                accountType = AccountIdClaimsType.Provider;
                return true;
            }
            else if (httpContextItems != null && httpContextItems.TryGetValue("EmployerAccountId", out var employerAccountIdValue) && long.TryParse(employerAccountIdValue as string, out var employerAccountId))
            {
                accountId = employerAccountId;
                accountType = AccountIdClaimsType.Employer;
                return true;
            }

            return false;
        }

        private static bool TryGetValidationRequired(IDictionary<object, object> httpContextItems, out bool validationRequired)
        {
            validationRequired = false;

            if (httpContextItems != null && httpContextItems.TryGetValue("IsClaimsValidationRequired", out var validationRequiredValue) && validationRequiredValue is bool)
            {
                validationRequired = (bool)validationRequiredValue;
                return true;
            }

            return false;
        }
    }
}