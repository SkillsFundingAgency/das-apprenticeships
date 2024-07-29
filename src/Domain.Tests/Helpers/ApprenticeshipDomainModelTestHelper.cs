using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using AutoFixture;
using System;
using System.Linq;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;
using SFA.DAS.Apprenticeships.Types;
using FundingPlatform = SFA.DAS.Apprenticeships.Enums.FundingPlatform;
using FundingType = SFA.DAS.Apprenticeships.Enums.FundingType;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Helpers
{
    public static class ApprenticeshipDomainModelTestHelper
    {
        private static readonly Fixture _fixture = new();

        public static ApprenticeshipDomainModel BuildApprenticeshipWithPendingStartDateChange(bool pendingProviderApproval = false)
        {
            _fixture.Customize(new ApprenticeshipCustomization());
            var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
            AddEpisode(apprenticeship);

            var startDateEntity = _fixture.Create<StartDateChange>();
            startDateEntity.PlannedEndDate = startDateEntity.ActualStartDate.AddMonths(24);
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

        public static void AddEpisode(ApprenticeshipDomainModel apprenticeship, DateTime? startDate = null, DateTime? endDate = null)
        {
            var start = startDate ?? _fixture.Create<DateTime>();
            var end = endDate ?? (start.AddDays(_fixture.Create<uint>()));
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

        public static void AddPendingPriceChangeEmployerInitiated(ApprenticeshipDomainModel apprenticeship, decimal? totalPrice = null, DateTime? effectiveFromDate = null)
        {
            apprenticeship.AddPriceHistory(
                null,
                null,
                totalPrice ?? _fixture.Create<int>(),
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

        public static void AddPendingStartDateChangeProviderInitiated(ApprenticeshipDomainModel apprenticeship, StartDateChange startDateChange)
        {
            apprenticeship.AddStartDateChange(
                startDateChange.ActualStartDate, 
                startDateChange.PlannedEndDate, 
                startDateChange.Reason, 
                startDateChange.ProviderApprovedBy, 
                startDateChange.ProviderApprovedDate, 
                null, 
                null, 
                startDateChange.CreatedDate, 
                startDateChange.RequestStatus, 
                ChangeInitiator.Provider);
        }

        public static void AddPendingStartDateChangeEmployerInitiated(ApprenticeshipDomainModel apprenticeship, StartDateChange startDateChange)
        {
            apprenticeship.AddStartDateChange(
                startDateChange.ActualStartDate, 
                startDateChange.PlannedEndDate, 
                startDateChange.Reason, 
                null, 
                null,  
                startDateChange.EmployerApprovedBy, 
                startDateChange.EmployerApprovedDate, 
                startDateChange.CreatedDate, 
                startDateChange.RequestStatus, 
                ChangeInitiator.Employer);
        }

        public static bool DoEpisodeDetailsMatchDomainModel(EpisodeUpdatedEvent e, ApprenticeshipDomainModel apprenticeship)
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
}
