using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Enums;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Apprenticeships.Infrastructure.UnitTests
{
    internal class WhenGettingAccountIdClaims
    {
        [Test]
        public void WithValidUkprns_ShouldReturnUkPrnClaims()
        {
            // Arrange
            var httpContextItems = new Dictionary<object, object>
            {
                { "Ukprn", "12345;67890" },
                { "IsClaimsValidationRequired", true }
            };
            var accountIdClaimsHandler = SetUpHandlerWithHttpContext(httpContextItems);

            // Act
            var accountIdClaims = accountIdClaimsHandler.GetAccountIdClaims();

            // Assert
            accountIdClaims.AccountIds.Count().Should().Be(2);
            accountIdClaims.AccountIds.Any(x => x == 12345).Should().BeTrue();
            accountIdClaims.AccountIds.Any(x => x == 67890).Should().BeTrue();
            accountIdClaims.AccountIdClaimsType.Should().Be(AccountIdClaimsType.Provider);
            accountIdClaims.IsClaimsValidationRequired.Should().BeTrue();
        }

        [Test]
        public void WithValidEmployerAccountId_ShouldReturnEmployerAccountIdClaims()
        {
            // Arrange
            var httpContextItems = new Dictionary<object, object>
            {
                { "EmployerAccountId", "98765;01010;66666" },
                { "IsClaimsValidationRequired", false }
            };

            var accountIdClaimsHandler = SetUpHandlerWithHttpContext(httpContextItems);

            // Act
            var accountIdClaims = accountIdClaimsHandler.GetAccountIdClaims();

            // Assert
            accountIdClaims.AccountIds.Count().Should().Be(3);
            accountIdClaims.AccountIds.Any(x => x == 98765).Should().BeTrue();
            accountIdClaims.AccountIds.Any(x => x == 01010).Should().BeTrue();
            accountIdClaims.AccountIds.Any(x => x == 66666).Should().BeTrue();
            accountIdClaims.AccountIdClaimsType.Should().Be(AccountIdClaimsType.Employer);
            accountIdClaims.IsClaimsValidationRequired.Should().BeFalse();
        }

        [TestCase("")]
        [TestCase("xxx;yyyy;invalid_value")]
        public void WithNoValidEmployerAccountIdClaimDataInHttpContext_ShouldReturnEmptyAccountIdClaims(string employerAccountIdValue)
        {
            // Arrange
            var httpContextItems = new Dictionary<object, object>
            {
                { "EmployerAccountId",  employerAccountIdValue}
            };
            var accountIdClaimsHandler = SetUpHandlerWithHttpContext(httpContextItems);

            // Act
            var accountIdClaims = accountIdClaimsHandler.GetAccountIdClaims();

            // Assert
            accountIdClaims.AccountIds.Should().BeNull();
            accountIdClaims.AccountIdClaimsType.Should().BeNull();
        }

        [TestCase("")]
        [TestCase("xxx;yyyy;invalid_value")]
        public void WithNoValidUkprnClaimDataInHttpContext_ShouldReturnEmptyAccountIdClaims(string ukprnValue)
        {
            // Arrange
            var httpContextItems = new Dictionary<object, object>
            {
                { "Ukprn",  ukprnValue}
            };
            var accountIdClaimsHandler = SetUpHandlerWithHttpContext(httpContextItems);

            // Act
            var accountIdClaims = accountIdClaimsHandler.GetAccountIdClaims();

            // Assert
            accountIdClaims.AccountIds.Should().BeNull();
            accountIdClaims.AccountIdClaimsType.Should().BeNull();
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
            accountIdClaims.AccountIds.Should().BeNull();
            accountIdClaims.AccountIdClaimsType.Should().BeNull();
            accountIdClaims.IsClaimsValidationRequired.Should().BeFalse();
        }

        private AccountIdClaimsHandler SetUpHandlerWithHttpContext(Dictionary<object, object> httpContextItems)
        {
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(x => x.HttpContext.Items).Returns(httpContextItems);
            var mockLogger = new Mock<ILogger<AccountIdClaimsHandler>>();
            return new AccountIdClaimsHandler(mockHttpContextAccessor.Object, mockLogger.Object);
        }
    }
}
