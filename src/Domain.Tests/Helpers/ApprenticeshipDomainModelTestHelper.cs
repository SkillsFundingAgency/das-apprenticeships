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

        public static void AddEpisode(ApprenticeshipDomainModel apprenticeship)
        {
            apprenticeship.AddEpisode(
                _fixture.Create<long>(),
                _fixture.Create<long>(), 
                _fixture.Create<DateTime>(), 
                _fixture.Create<DateTime>(), 
                _fixture.Create<decimal>(),
                _fixture.Create<decimal>(),
                _fixture.Create<decimal>(),
                _fixture.Create<FundingType>(),
                _fixture.Create<FundingPlatform?>(),
                _fixture.Create<int>(),
                _fixture.Create<long?>(),
                _fixture.Create<string>(),
                _fixture.Create<long>(),
                _fixture.Create<string>(),
                _fixture.Create<string?>());
        }

        public static void AddPendingPriceChange(ApprenticeshipDomainModel apprenticeship)
        {
            apprenticeship.AddPriceHistory(
                _fixture.Create<decimal>(), 
                _fixture.Create<decimal>(), 
                _fixture.Create<decimal>(), 
                _fixture.Create<DateTime>(), 
                _fixture.Create<DateTime>(), 
                ChangeRequestStatus.Created, 
                null, 
                _fixture.Create<string>(), 
                _fixture.Create<string>(), 
                _fixture.Create<DateTime>(), 
                _fixture.Create<DateTime>(), 
                _fixture.Create<ChangeInitiator>());
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
