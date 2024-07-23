using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using AutoFixture;
using System;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Helpers
{
    public static class ApprenticeshipDomainModelTestHelper
    {
        private static readonly Fixture _fixture = new();

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

        public static void AddPendingStartDateChange(ApprenticeshipDomainModel apprenticeship, StartDateChange startDateChange)
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
    }
}
