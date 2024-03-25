using System;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.Infrastructure;

namespace SFA.DAS.Apprenticeships.DataAccess.UnitTests
{
    public class AccountIdAuthorizerTests
    {
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
        }

        [Test]
        public void ValidateAccountIds_ProviderAccountIdMismatch_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var apprenticeship = _fixture.Create<Apprenticeship>();
            apprenticeship.Ukprn = 12345;
            var authorizer = SetUpAuthorizer(true, 54321, AccountIdClaimsType.Provider);

            // Act 
            var act = () => authorizer.ValidateAccountIds(apprenticeship);
            
            // Assert
            act.Should().Throw<UnauthorizedAccessException>();
        }

        [Test]
        public void ValidateAccountIds_EmployerAccountIdMismatch_ThrowsUnauthorizedAccessException()
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
        public void ValidateAccountIds_NoClaimsValidationRequired_DoesNotThrowException()
        {
            // Arrange
            var apprenticeship = new Apprenticeship();
            var authorizer = SetUpAuthorizer(false);

            // Act 
            var act = () => authorizer.ValidateAccountIds(apprenticeship);
            
            // Assert
            act.Should().NotThrow();
        }
        
        private AccountIdAuthorizer SetUpAuthorizer(bool isClaimsValidationRequired, long? accountId = null, AccountIdClaimsType? accountIdClaimsType = null)
        {
            var mockClaimsHandler = new Mock<IAccountIdClaimsHandler>();
            mockClaimsHandler
                .Setup(x => x.GetAccountIdClaims())
                .Returns(new AccountIdClaims
                {
                    IsClaimsValidationRequired = isClaimsValidationRequired, AccountId = accountId, AccountIdClaimsType = accountIdClaimsType
                });
            return new AccountIdAuthorizer(mockClaimsHandler.Object);
        }

    }
}
