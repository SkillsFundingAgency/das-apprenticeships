using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Enums;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Apprenticeships.Infrastructure.UnitTests
{
    internal class WhenGettingAccountIdClaims
    {
        [Test]
        public void WithValidUkprn_ShouldReturnUkPrnClaims()
        {
            // Arrange
            var httpContextItems = new Dictionary<object, object>
            {
                { "Ukprn", "12345" },
                { "IsClaimsValidationRequired", true }
            };
            var accountIdClaimsHandler = SetUpHandlerWithHttpContext(httpContextItems);

            // Act
            var accountIdClaims = accountIdClaimsHandler.GetAccountIdClaims();

            // Assert
            accountIdClaims.AccountId.Should().Be(12345);
            accountIdClaims.AccountIdClaimsType.Should().Be(AccountIdClaimsType.Provider);
            accountIdClaims.IsClaimsValidationRequired.Should().BeTrue();
        }

        private AccountIdClaimsHandler SetUpHandlerWithHttpContext(Dictionary<object, object> httpContextItems)
        {
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(x => x.HttpContext.Items).Returns(httpContextItems);
            var mockLogger = new Mock<ILogger<AccountIdClaimsHandler>>();
            return new AccountIdClaimsHandler(mockHttpContextAccessor.Object, mockLogger.Object);
        }

        [Test]
        public void WithValidEmployerAccountId_ShouldReturnEmployerAccountIdClaims()
        {
            // Arrange
            var httpContextItems = new Dictionary<object, object>
            {
                { "EmployerAccountId", "98765" },
                { "IsClaimsValidationRequired", false }
            };

            var accountIdClaimsHandler = SetUpHandlerWithHttpContext(httpContextItems);

            // Act
            var accountIdClaims = accountIdClaimsHandler.GetAccountIdClaims();

            // Assert
            accountIdClaims.AccountId.Should().Be(98765);
            accountIdClaims.AccountIdClaimsType.Should().Be(AccountIdClaimsType.Employer);
            accountIdClaims.IsClaimsValidationRequired.Should().BeFalse();
        }

        [Test]
        public void WithNoClaimDataInHttpContext_ShouldReturnEmptyAccountIdClaims()
        {
            // Arrange
            var accountIdClaimsHandler = SetUpHandlerWithHttpContext(null);

            // Act
            var accountIdClaims = accountIdClaimsHandler.GetAccountIdClaims();

            // Assert
            accountIdClaims.AccountId.Should().Be(null);
            accountIdClaims.AccountIdClaimsType.Should().Be(null);
            accountIdClaims.IsClaimsValidationRequired.Should().BeFalse();
        }
    }
}
