using AutoFixture;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;
using System.Reflection;
using SFA.DAS.Learning.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Learning.Domain.Apprenticeship;
using SFA.DAS.Learning.Enums;
using SFA.DAS.Learning.Types;
using FundingPlatform = SFA.DAS.Learning.Enums.FundingPlatform;
using FundingType = SFA.DAS.Learning.Enums.FundingType;

namespace SFA.DAS.Apprenticeships.TestHelpers;

public static class ApprenticeshipDomainModelTestHelper
{
    private static readonly Fixture _fixture = new();

    // If this method isn't a sign that we need to refactor this project then I don't know what is
    public static ApprenticeshipDomainModel CreateBasicTestModel()
    {
        // Create an instance with default constructor or Activator
        var apprenticeship = (ApprenticeshipDomainModel)Activator.CreateInstance(
            typeof(ApprenticeshipDomainModel),
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            new object[] { new Apprenticeship() },
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

    public static void AddEpisode(ApprenticeshipDomainModel apprenticeship, DateTime? startDate = null, DateTime? endDate = null, long? ukprn = null, FundingPlatform? fundingPlatform = FundingPlatform.DAS)
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
            fundingPlatform,
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

    public static ApprenticeshipDomainModel BuildApprenticeshipWithPendingStartDateChange(
    bool pendingProviderApproval = false,
    DateTime? originalStartDate = null,
    DateTime? newStartDate = null,
    DateTime? originalEndDate = null,
    DateTime? newEndDate = null)
    {
        _fixture.Customize(new ApprenticeshipCustomization());
        var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
        ApprenticeshipDomainModelTestHelper.AddEpisode(apprenticeship, originalStartDate, originalEndDate);

        var startDateEntity = _fixture.Build<StartDateChange>().With(x => x.ActualStartDate, newStartDate ?? _fixture.Create<DateTime>()).Create();
        startDateEntity.PlannedEndDate = newEndDate ?? startDateEntity.ActualStartDate.AddMonths(24);
        var startDateChange = StartDateChangeDomainModel.Get(startDateEntity);

        if (pendingProviderApproval)
        {
            apprenticeship.AddStartDateChange(startDateChange.ActualStartDate, startDateChange.PlannedEndDate, startDateChange.Reason,
                null, null,
                startDateChange.EmployerApprovedBy, startDateChange.EmployerApprovedDate, startDateChange.CreatedDate,
                ChangeRequestStatus.Created, ChangeInitiator.Employer);
        }
        else
        {
            apprenticeship.AddStartDateChange(startDateChange.ActualStartDate, startDateChange.PlannedEndDate, startDateChange.Reason,
                startDateChange.ProviderApprovedBy, startDateChange.ProviderApprovedDate,
                null, null, startDateChange.CreatedDate,
                ChangeRequestStatus.Created, ChangeInitiator.Provider);
        }

        return apprenticeship;
    }

    public static bool DoEpisodeDetailsMatchDomainModel(ApprenticeshipEvent e, ApprenticeshipDomainModel apprenticeship)
    {
        var episode = apprenticeship.LatestEpisode;
        var expectedNumberOfPrices = apprenticeship.AllPrices.Count();
        var episodePrice = apprenticeship.LatestPrice;
        return
            e.Episode.TrainingCode == episode.TrainingCode &&
            e.Episode.FundingEmployerAccountId == episode.FundingEmployerAccountId &&
            e.Episode.EmployerAccountId == episode.EmployerAccountId &&
            e.Episode.LegalEntityName == episode.LegalEntityName &&
            e.Episode.Ukprn == episode.Ukprn &&
            e.Episode.AgeAtStartOfApprenticeship == apprenticeship.AgeAtStartOfApprenticeship &&
            e.Episode.Prices.Count == expectedNumberOfPrices &&
            e.Episode.Prices.MaxBy(x => x.StartDate).TotalPrice == episodePrice.TotalPrice &&
            e.Episode.FundingType == episode.FundingType &&
            e.Episode.Prices.MaxBy(x => x.StartDate).StartDate == episodePrice.StartDate &&
            e.Episode.Prices.MaxBy(x => x.StartDate).EndDate == episodePrice.EndDate &&
            e.Episode.FundingPlatform == episode.FundingPlatform;
    }
}
