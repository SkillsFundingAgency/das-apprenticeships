using System;
using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Learning.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Learning.Enums;

namespace SFA.DAS.Learning.DataAccess.UnitTests
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
            var apprenticeship = AccountIdAuthorizerTestHelper.BuildApprenticeshipWithAccountId(12345);
            var idsInClaims = new List<long> { 54321, 98765 };
            var authorizer = AccountIdAuthorizerTestHelper.SetUpAuthorizer(true, idsInClaims, AccountIdClaimsType.Provider);

            // Act 
            Action act = () => authorizer.AuthorizeAccountId(apprenticeship);

            // Assert
            act.Should().Throw<UnauthorizedAccessException>();
        }

        [Test]
        public void EmployerAccountIdMismatch_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var apprenticeship = AccountIdAuthorizerTestHelper.BuildApprenticeshipWithAccountId(null, 98765);

            var idsInClaims = new List<long> { 5534534321, 0129843 };
            var authorizer = AccountIdAuthorizerTestHelper.SetUpAuthorizer(true, idsInClaims, AccountIdClaimsType.Employer);

            // Act 
            Action act = () => authorizer.AuthorizeAccountId(apprenticeship);

            // Assert
            act.Should().Throw<UnauthorizedAccessException>();
        }

        [Test]
        public void NoClaimsValidationRequired_DoesNotThrowException()
        {
            // Arrange
            var apprenticeship = new Apprenticeship();
            var authorizer = AccountIdAuthorizerTestHelper.SetUpAuthorizer(false);

            // Act 
            Action act = () => authorizer.AuthorizeAccountId(apprenticeship);

            // Assert
            act.Should().NotThrow();
        }

        [Test]
        public void ValidEmployerAccountId_DoesNothing()
        {
            // Arrange
            var apprenticeship = AccountIdAuthorizerTestHelper.BuildApprenticeshipWithAccountId(null, 98765);
            var idsInClaims = new List<long> { 98765, 0129843 };
            var authorizer = AccountIdAuthorizerTestHelper.SetUpAuthorizer(true, idsInClaims, AccountIdClaimsType.Employer);

            // Act 
            Action act = () => authorizer.AuthorizeAccountId(apprenticeship);

            // Assert
            act.Should().NotThrow();
        }

        [Test]
        public void ValidUkPrn_DoesNothing()
        {
            // Arrange
            var apprenticeship = AccountIdAuthorizerTestHelper.BuildApprenticeshipWithAccountId(54321, null);
            var idsInClaims = new List<long> { 98765, 54321 };
            var authorizer = AccountIdAuthorizerTestHelper.SetUpAuthorizer(true, idsInClaims, AccountIdClaimsType.Provider);

            // Act 
            Action act = () => authorizer.AuthorizeAccountId(apprenticeship);

            // Assert
            act.Should().NotThrow();
        }
    }
}
