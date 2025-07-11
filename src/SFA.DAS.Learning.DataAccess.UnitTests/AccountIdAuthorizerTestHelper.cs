using System;
using System.Collections.Generic;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Learning.DataAccess.Entities.Learning;
using SFA.DAS.Learning.Enums;
using SFA.DAS.Learning.Infrastructure;

namespace SFA.DAS.Learning.DataAccess.UnitTests;

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

    public static Entities.Learning.Learning BuildApprenticeshipWithAccountId(long? ukprn = null, long? employerAccountId = null)
    {
        var fixture = new Fixture();

        return new Entities.Learning.Learning
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