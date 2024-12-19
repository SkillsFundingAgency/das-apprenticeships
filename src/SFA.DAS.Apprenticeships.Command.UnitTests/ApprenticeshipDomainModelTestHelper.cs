using System;
using System.Collections.Generic;
using System.Reflection;
using AutoFixture;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.Command.UnitTests;

public static class ApprenticeshipDomainModelTestHelper
{
    private static readonly Fixture _fixture = new();

    // If this method isn't a sign that we need to refactor this project then I don't know what is
    internal static ApprenticeshipDomainModel CreateBasicTestModel()
    {
        // Create an instance with default constructor or Activator
        var apprenticeship = (ApprenticeshipDomainModel)Activator.CreateInstance(
            typeof(ApprenticeshipDomainModel),
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            new object[] { new DataAccess.Entities.Apprenticeship.Apprenticeship(), false },
            null
        );

        // Set private fields to empty lists using reflection
        typeof(ApprenticeshipDomainModel)
            .GetField("_episodes", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(apprenticeship, new List<EpisodeDomainModel>());

        typeof(ApprenticeshipDomainModel)
            .GetField("_priceHistories", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(apprenticeship, new List<PriceHistoryDomainModel>());

        typeof(ApprenticeshipDomainModel)
            .GetField("_startDateChanges", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(apprenticeship, new List<StartDateChangeDomainModel>());

        typeof(ApprenticeshipDomainModel)
            .GetField("_freezeRequests", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(apprenticeship, new List<FreezeRequestDomainModel>());

        return apprenticeship;
    }

    public static void AddEpisode(ApprenticeshipDomainModel apprenticeship, DateTime? startDate = null, DateTime? endDate = null, long? ukprn = null)
    {
        var start = startDate ?? _fixture.Create<DateTime>();
        var end = endDate ?? (start.AddDays(_fixture.Create<int>()));
        var ukprnValue = ukprn ?? _fixture.Create<long>();

        apprenticeship.AddEpisode(
            ukprnValue,
            _fixture.Create<long>(), 
            start,
            end,
            _fixture.Create<decimal>(),
            _fixture.Create<decimal>(),
            _fixture.Create<decimal>(),
            _fixture.Create<FundingType>(),
            _fixture.Create<FundingPlatform?>(),
            _fixture.Create<int>(),
            _fixture.Create<long?>(),
            _fixture.Create<string>(),
            _fixture.Create<long>(),
            _fixture.Create<int>().ToString(),
            _fixture.Create<string?>());
    }

    public static void AddPendingPriceChangeEmployerInitiated(ApprenticeshipDomainModel apprenticeship, decimal totalPrice, DateTime? effectiveFromDate = null)
    {
        apprenticeship.AddPriceHistory(
            null,
            null,
            totalPrice,
            effectiveFromDate ?? _fixture.Create<DateTime>(),
            _fixture.Create<DateTime>(),
            ChangeRequestStatus.Created,
            null,
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            null,
            _fixture.Create<DateTime>(),
            ChangeInitiator.Employer);
    }

    public static void AddPendingPriceChangeProviderInitiated(ApprenticeshipDomainModel apprenticeship, DateTime? effectiveFromDate = null)
    {
        apprenticeship.AddPriceHistory(
            _fixture.Create<decimal>(), 
            _fixture.Create<decimal>(), 
            _fixture.Create<decimal>(), 
            effectiveFromDate ?? _fixture.Create<DateTime>(),
            _fixture.Create<DateTime>(), 
            ChangeRequestStatus.Created, 
            _fixture.Create<string>(), 
            _fixture.Create<string>(), 
            null, 
            _fixture.Create<DateTime>(), 
            null, 
            ChangeInitiator.Provider);
    }

    public static void AddPendingStartDateChange(ApprenticeshipDomainModel apprenticeship, ChangeInitiator changeInitiator, DateTime? startDate = null)
    {
        var start = startDate ?? _fixture.Create<DateTime>();
        var end = start.AddDays(_fixture.Create<int>());
        apprenticeship.AddStartDateChange(
            start,
            end,
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<DateTime>(),
            null,
            null,
            _fixture.Create<DateTime>(),
            ChangeRequestStatus.Created,
            changeInitiator);
    }
}