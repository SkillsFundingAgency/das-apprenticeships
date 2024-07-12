﻿using AutoFixture;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Apprenticeships.DataAccess;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.TestHelpers;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Helpers;

public static class ApprenticeshipDbContextTestHelper
{
    private static readonly Fixture _fixture = new();

    public static ApprenticeshipQueryRepository SetUpApprenticeshipQueryRepository(this ApprenticeshipsDataContext dbContext)
    {
        dbContext = InMemoryDbContextCreator.SetUpInMemoryDbContext();
        var logger = Mock.Of<ILogger<ApprenticeshipQueryRepository>>();
        return new ApprenticeshipQueryRepository(new Lazy<ApprenticeshipsDataContext>(dbContext), logger);
    }

    public static async Task<DataAccess.Entities.Apprenticeship.Apprenticeship> AddApprenticeship(
        this ApprenticeshipsDataContext dbContext, 
        Guid apprenticeshipKey, 
        bool addPendingPriceHistoryRequest,
        long? ukprn = null,
        string? initiator = null,
        long? approvalsApprenticeshipId = null,
        FundingPlatform? fundingPlatform = null,
        DateTime? startDate = null)
    {
        var episodeKey = _fixture.Create<Guid>();
        var episodePrice = _fixture.Build<EpisodePrice>()
            .With(x => x.EpisodeKey, episodeKey)
            .With(x => x.StartDate, startDate ?? _fixture.Create<DateTime>())
            .With(x => x.IsDeleted, false)
            .Create();

        var episode = _fixture.Build<Episode>()
            .With(x => x.ApprenticeshipKey, apprenticeshipKey)
            .With(x => x.Key, episodeKey)
            .With(x => x.Ukprn, ukprn ?? _fixture.Create<long>())
            .With(x => x.FundingPlatform, fundingPlatform ?? _fixture.Create<FundingPlatform>())
            .With(x => x.Prices, new List<EpisodePrice> { episodePrice })
            .Create();

        var apprenticeship = _fixture.Build<DataAccess.Entities.Apprenticeship.Apprenticeship>()
            .With(x => x.Key, apprenticeshipKey)
            .With(x => x.ApprovalsApprenticeshipId, approvalsApprenticeshipId ?? _fixture.Create<long>())
            .With(x => x.Episodes, new List<Episode>() { episode })
            .Create();

        if (addPendingPriceHistoryRequest)
        {
            var providerApprovedDate = initiator == "Provider" ? _fixture.Create<DateTime>() : (DateTime?)null;
            var employerApprovedDate = initiator == "Employer" ? _fixture.Create<DateTime>() : (DateTime?)null;

            apprenticeship.PriceHistories = new List<PriceHistory>()
            {
                new()
                {
                    Key = _fixture.Create<Guid>(),
                    ApprenticeshipKey = apprenticeshipKey,
                    PriceChangeRequestStatus = ChangeRequestStatus.Created,
                    TrainingPrice = 10000,
                    AssessmentPrice = 3000,
                    TotalPrice = 13000,
                    EffectiveFromDate = _fixture.Create<DateTime>(),
                    ChangeReason = "testReason",
                    ProviderApprovedDate = providerApprovedDate,
                    EmployerApprovedDate = employerApprovedDate,
                    ProviderApprovedBy = initiator == "Provider" ? "Mr Provider" : null,
                    EmployerApprovedBy = initiator == "Employer" ? "Mr Employer" : null,
                    Initiator = initiator == "Employer" ? ChangeInitiator.Employer : ChangeInitiator.Provider
                }
            };
        }

        await dbContext.AddAsync(apprenticeship);
        await dbContext.SaveChangesAsync();
        return apprenticeship;
    }
}