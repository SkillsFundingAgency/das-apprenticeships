using System;
using System.Collections.Generic;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.Infrastructure;

namespace SFA.DAS.Apprenticeships.DataAccess.UnitTests;

public static class AccountIdAuthorizerTestHelper
{
    public static AccountIdAuthorizer SetUpAuthorizer(bool isClaimsValidationRequired, List<long>? accountIds = null, AccountIdClaimsType? accountIdClaimsType = null)
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

    public static Apprenticeship BuildApprenticeshipWithAccountId(long? ukprn = null, long? employerAccountId = null)
    {
        var fixture = new Fixture();

        return new Apprenticeship
        {
            Episodes = new List<Episode>
            {
                //current episode to be validated against
                new()
                {
                    Ukprn = ukprn ?? fixture.Create<long>(),
                    EmployerAccountId = employerAccountId ?? fixture.Create<long>(),
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
                    Ukprn = fixture.Create<long>(),
                    EmployerAccountId = fixture.Create<long>(),
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