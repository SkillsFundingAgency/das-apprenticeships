using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using SFA.DAS.Encoding;
using AutoFixture;
using SFA.DAS.Learning.Enums;
using SFA.DAS.Learning.Infrastructure;

namespace SFA.DAS.Apprenticeships.Infrastructure.UnitTests
{
    internal class WhenGettingAccountIdClaims
    {
        private readonly Fixture _fixture;

        public WhenGettingAccountIdClaims()
        {
            _fixture = new Fixture();
        }

        [Test]
        public void WithValidUkprns_ShouldReturnUkPrnClaims()
        {
            // Arrange
            var httpContextItems = new Dictionary<object, object>
            {
                { "Ukprn", "12345;034754" },
                { "IsClaimsValidationRequired", true }
            };
            var accountIdClaimsHandler = SetUpHandlerWithHttpContext(httpContextItems, new Mock<IEncodingService>());

            // Act
            var accountIdClaims = accountIdClaimsHandler.GetAccountIdClaims();

            // Assert
            accountIdClaims.AccountIds.Count().Should().Be(2);
            accountIdClaims.AccountIds.Any(x => x == 12345).Should().BeTrue();
            accountIdClaims.AccountIds.Any(x => x == 034754).Should().BeTrue();
            accountIdClaims.AccountIdClaimsType.Should().Be(AccountIdClaimsType.Provider);
            accountIdClaims.IsClaimsValidationRequired.Should().BeTrue();
        }

        [Test]
        public void WithValidEmployerAccountId_ShouldReturnEmployerAccountIdClaims()
        {
            // Arrange
            var httpContextItems = new Dictionary<object, object>
            {
                { "EmployerAccountId", "98765;invalid_id;66666" },
                { "IsClaimsValidationRequired", false }
            };

            var mockEncodingService = new Mock<IEncodingService>();
            var decodedValue1 = _fixture.Create<long>();
            var invalidDecodedValue2 = _fixture.Create<long>();
            var decodedValue3 = _fixture.Create<long>();
            mockEncodingService.Setup(x => x.TryDecode("98765", EncodingType.AccountId, out decodedValue1)).Returns(true);
            mockEncodingService.Setup(x => x.TryDecode("invalid_id", EncodingType.AccountId, out invalidDecodedValue2)).Returns(false);
            mockEncodingService.Setup(x => x.TryDecode("66666", EncodingType.AccountId, out decodedValue3)).Returns(true);
            var accountIdClaimsHandler = SetUpHandlerWithHttpContext(httpContextItems, mockEncodingService);

            // Act
            var accountIdClaims = accountIdClaimsHandler.GetAccountIdClaims();

            // Assert
            accountIdClaims.AccountIds.Count().Should().Be(2);
            accountIdClaims.AccountIds.Any(x => x == decodedValue1).Should().BeTrue();
            accountIdClaims.AccountIds.Any(x => x == invalidDecodedValue2).Should().BeFalse();
            accountIdClaims.AccountIds.Any(x => x == decodedValue3).Should().BeTrue();
            accountIdClaims.AccountIdClaimsType.Should().Be(AccountIdClaimsType.Employer);
            accountIdClaims.IsClaimsValidationRequired.Should().BeFalse();
        }

        [TestCase("")]
        [TestCase("xxx;yyyy;invalid_value")]
        public void WithInvalidEmployerAccountIdClaimDataInHttpContext_ShouldReturnEmptyAccountIdClaims(string employerAccountIdValue)
        {
            // Arrange
            var httpContextItems = new Dictionary<object, object>
            {
                { "EmployerAccountId",  employerAccountIdValue}
            };
            var mockEncodingService = new Mock<IEncodingService>();
            var decodedValue1 = _fixture.Create<long>();
            var decodedValue2 = _fixture.Create<long>();
            var decodedValue3 = _fixture.Create<long>();
            mockEncodingService.Setup(x => x.TryDecode("xxx", EncodingType.AccountId, out decodedValue1)).Returns(false);
            mockEncodingService.Setup(x => x.TryDecode("yyyy", EncodingType.AccountId, out decodedValue2)).Returns(false);
            mockEncodingService.Setup(x => x.TryDecode("invalid_value", EncodingType.AccountId, out decodedValue3)).Returns(false);
            var accountIdClaimsHandler = SetUpHandlerWithHttpContext(httpContextItems, mockEncodingService);

            // Act
            var accountIdClaims = accountIdClaimsHandler.GetAccountIdClaims();

            // Assert
            accountIdClaims.AccountIds.Should().BeNull();
            accountIdClaims.AccountIdClaimsType.Should().BeNull();
        }

        [TestCase("")]
        [TestCase("xxx;yyyy;invalid_value")]
        public void WithInvalidUkprnClaimDataInHttpContext_ShouldReturnEmptyAccountIdClaims(string ukprnValue)
        {
            // Arrange
            var httpContextItems = new Dictionary<object, object>
            {
                { "Ukprn",  ukprnValue}
            };
            var accountIdClaimsHandler = SetUpHandlerWithHttpContext(httpContextItems, new Mock<IEncodingService>());

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
            var accountIdClaimsHandler = SetUpHandlerWithHttpContext(null, new Mock<IEncodingService>());

            // Act
            var accountIdClaims = accountIdClaimsHandler.GetAccountIdClaims();

            // Assert
            accountIdClaims.AccountIds.Should().BeNull();
            accountIdClaims.AccountIdClaimsType.Should().BeNull();
            accountIdClaims.IsClaimsValidationRequired.Should().BeFalse();
        }

        private AccountIdClaimsHandler SetUpHandlerWithHttpContext(Dictionary<object, object> httpContextItems, Mock<IEncodingService> mockEncodingService)
        {
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(x => x.HttpContext.Items).Returns(httpContextItems);
            var mockLogger = new Mock<ILogger<AccountIdClaimsHandler>>();
            return new AccountIdClaimsHandler(mockHttpContextAccessor.Object, mockLogger.Object, mockEncodingService.Object);
        }
    }
}
