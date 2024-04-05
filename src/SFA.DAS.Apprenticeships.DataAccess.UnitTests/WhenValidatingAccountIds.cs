using System;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.Infrastructure;

namespace SFA.DAS.Apprenticeships.DataAccess.UnitTests
{
    public class WhenValidatingAccountIds
    {
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
        }

        [Test]
        public void ProviderAccountIdMismatch_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var apprenticeship = _fixture.Build<Apprenticeship>().With(x => x.Ukprn, 12345).Create();
            var authorizer = SetUpAuthorizer(true, 54321, AccountIdClaimsType.Provider);

            // Act 
            var act = () => authorizer.ValidateAccountIds(apprenticeship);
            
            // Assert
            act.Should().Throw<UnauthorizedAccessException>();
        }

        [Test]
        public void EmployerAccountIdMismatch_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var apprenticeship = new Apprenticeship { EmployerAccountId = 98765 };
            var authorizer = SetUpAuthorizer(true, 56789, AccountIdClaimsType.Employer);

            // Act 
            var act = () => authorizer.ValidateAccountIds(apprenticeship);
            
            // Assert
            act.Should().Throw<UnauthorizedAccessException>();
        }

        [Test]
        public void NoClaimsValidationRequired_DoesNotThrowException()
        {
            // Arrange
            var apprenticeship = new Apprenticeship();
            var authorizer = SetUpAuthorizer(false);

            // Act 
            var act = () => authorizer.ValidateAccountIds(apprenticeship);
            
            // Assert
            act.Should().NotThrow();
        }

        [TestCase()]
        public void ValidEmployerAccountId_DoesNothing()
        {
            // Arrange
            var apprenticeship = new Apprenticeship { EmployerAccountId = 98765 };
            var authorizer = SetUpAuthorizer(true, 98765, AccountIdClaimsType.Employer);

            // Act 
            var act = () => authorizer.ValidateAccountIds(apprenticeship);
    
            // Assert
            act.Should().NotThrow();
        }

        [Test]
        public void ValidUkPrn_DoesNothing()
        {
            // Arrange
            var apprenticeship = new Apprenticeship { Ukprn = 54321 };
            var authorizer = SetUpAuthorizer(true, 54321, AccountIdClaimsType.Provider);

            // Act 
            var act = () => authorizer.ValidateAccountIds(apprenticeship);
    
            // Assert
            act.Should().NotThrow();
        }

        private static AccountIdAuthorizer SetUpAuthorizer(bool isClaimsValidationRequired, long? accountId = null, AccountIdClaimsType? accountIdClaimsType = null)
        {
            var mockClaimsHandler = new Mock<IAccountIdClaimsHandler>();
            mockClaimsHandler
                .Setup(x => x.GetAccountIdClaims())
                .Returns(new AccountIdClaims
                {
                    IsClaimsValidationRequired = isClaimsValidationRequired, AccountId = accountId, AccountIdClaimsType = accountIdClaimsType
                });
            var mockLogger = new Mock<ILogger<AccountIdAuthorizer>>();
            return new AccountIdAuthorizer(mockClaimsHandler.Object, mockLogger.Object);
        }
    }
}
