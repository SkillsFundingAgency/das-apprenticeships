using System;
using AutoFixture;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.Command.UnitTests;

public static class ApprenticeshipDomainModelTestHelper
{
    private static readonly Fixture _fixture = new();

    public static void AddEpisode(ApprenticeshipDomainModel apprenticeship, DateTime? startDate = null, DateTime? endDate = null)
    {
        var start = startDate ?? _fixture.Create<DateTime>();
        var end = endDate ?? (start.AddDays(_fixture.Create<int>()));
        apprenticeship.AddEpisode(
            _fixture.Create<long>(),
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