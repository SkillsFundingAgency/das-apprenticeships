using System;
using System.Collections.Generic;
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
            var apprenticeship = BuildApprenticeshipWithAccountId(12345);
            var idsInClaims = new List<long> { 54321, 98765 };
            var authorizer = SetUpAuthorizer(true, idsInClaims, AccountIdClaimsType.Provider);

            // Act 
            Action act = () => authorizer.AuthorizeAccountId(apprenticeship);

            // Assert
            act.Should().Throw<UnauthorizedAccessException>();
        }

        [Test]
        public void EmployerAccountIdMismatch_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var apprenticeship = BuildApprenticeshipWithAccountId(null, 98765);

            var idsInClaims = new List<long> { 5534534321, 0129843 };
            var authorizer = SetUpAuthorizer(true, idsInClaims, AccountIdClaimsType.Employer);

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
            var authorizer = SetUpAuthorizer(false);

            // Act 
            Action act = () => authorizer.AuthorizeAccountId(apprenticeship);

            // Assert
            act.Should().NotThrow();
        }

        [Test]
        public void ValidEmployerAccountId_DoesNothing()
        {
            // Arrange
            var apprenticeship = BuildApprenticeshipWithAccountId(null, 98765);
            var idsInClaims = new List<long> { 98765, 0129843 };
            var authorizer = SetUpAuthorizer(true, idsInClaims, AccountIdClaimsType.Employer);

            // Act 
            Action act = () => authorizer.AuthorizeAccountId(apprenticeship);

            // Assert
            act.Should().NotThrow();
        }

        [Test]
        public void ValidUkPrn_DoesNothing()
        {
            // Arrange
            var apprenticeship = BuildApprenticeshipWithAccountId(54321, null);
            var idsInClaims = new List<long> { 98765, 54321 };
            var authorizer = SetUpAuthorizer(true, idsInClaims, AccountIdClaimsType.Provider);

            // Act 
            Action act = () => authorizer.AuthorizeAccountId(apprenticeship);

            // Assert
            act.Should().NotThrow();
        }

        private static AccountIdAuthorizer SetUpAuthorizer(bool isClaimsValidationRequired, List<long>? accountIds = null, AccountIdClaimsType? accountIdClaimsType = null)
        {
            var mockClaimsHandler = new Mock<IAccountIdClaimsHandler>();
            mockClaimsHandler
                .Setup(x => x.GetAccountIdClaims())
                .Returns(new AccountIdClaims
                {
                    IsClaimsValidationRequired = isClaimsValidationRequired,
                    AccountIds = accountIds,
                    AccountIdClaimsType = accountIdClaimsType
                });
            var mockLogger = new Mock<ILogger<AccountIdAuthorizer>>();
            return new AccountIdAuthorizer(mockClaimsHandler.Object, mockLogger.Object);
        }

        private Apprenticeship BuildApprenticeshipWithAccountId(long? ukprn = null, long? employerAccountId = null)
        {
            return new Apprenticeship
            {
                Episodes = new List<Episode>
                {
                    //current episode to be validated against
                    new()
                    {
                        Ukprn = ukprn ?? _fixture.Create<long>(),
                        EmployerAccountId = employerAccountId ?? _fixture.Create<long>(),
                        Prices = new List<EpisodePrice>
                        {
                            new()
                            {
                                StartDate = DateTime.UtcNow.AddDays(-2),
                                EndDate = DateTime.UtcNow.AddDays(2)
                            }
                        }
                    },
                    //other episode with non-matching ids no to be validated against
                    new()
                    {
                        Ukprn = _fixture.Create<long>(),
                        EmployerAccountId = _fixture.Create<long>(),
                        Prices = new List<EpisodePrice>
                        {
                            new()
                            {
                                StartDate = DateTime.UtcNow.AddDays(-100),
                                EndDate = DateTime.UtcNow.AddDays(-2)
                            }
                        }
                    }
                }
            };
        }
    }
}
