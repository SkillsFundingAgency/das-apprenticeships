using Microsoft.AspNetCore.Http;
using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.Infrastructure
{
    public class AccountIdClaimsHandler : IAccountIdClaimsHandler
    {
        private readonly HttpContext _httpContextAccessor;

        public AccountIdClaimsHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor.HttpContext;
        }

        public AccountIdClaims GetAccountIdClaims()
        {
            var httpContextItems = _httpContextAccessor.Items;
            var accountIdClaims = new AccountIdClaims();

            var validationRequired = VerifyIfValidationRequired(httpContextItems, accountIdClaims);
            if (!validationRequired)
            {
                return accountIdClaims;
            }

            //todo: figure out why items has a 0 ukprn value by default?
            if (httpContextItems != null && httpContextItems.TryGetValue("Ukprn", out var ukprnValue) && long.TryParse(ukprnValue as string, out var ukprn))
            {
                accountIdClaims.AccountId = ukprn;
                accountIdClaims.AccountIdClaimsType = AccountIdClaimsType.Provider;
            }
            else if (httpContextItems != null && httpContextItems.TryGetValue("EmployerAccountId", out var employerAccountIdValue) && long.TryParse(employerAccountIdValue as string, out var employerAccountId))
            {
                accountIdClaims.AccountId = employerAccountId;
                accountIdClaims.AccountIdClaimsType = AccountIdClaimsType.Employer;
            }

            return accountIdClaims;
        }

        private static bool VerifyIfValidationRequired(IDictionary<object, object> httpContextItems, AccountIdClaims accountIdClaims)
        {
            var valueExists = httpContextItems.TryGetValue("IsClaimsValidationRequired", out var validationRequiredValue);
            var validationRequired = validationRequiredValue is bool ? (bool)validationRequiredValue : false;
            
            accountIdClaims.IsClaimsValidationRequired = validationRequired;
            return validationRequired;
        }
    }
}
