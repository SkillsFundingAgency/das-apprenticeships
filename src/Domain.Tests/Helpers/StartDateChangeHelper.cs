using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Factories;
using SFA.DAS.Apprenticeships.Enums;
using System;
using AutoFixture;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Helpers
{
    public static class StartDateChangeTestHelper
    {
        private static readonly Fixture Fixture = new();

        public static ApprenticeshipDomainModel BuildApprenticeshipWithPendingStartDateChange(bool pendingProviderApproval = false)
        {
            var apprenticeship = new ApprenticeshipFactory().CreateNew(
                "1234435",
                "TRN",
                new DateTime(2000,
                    10,
                    16),
                "Ron",
                "Swanson",
                Fixture.Create<decimal?>(),
                Fixture.Create<decimal?>(),
                Fixture.Create<decimal>(),
                Fixture.Create<string>(),
                Fixture.Create<int>(),
                Fixture.Create<DateTime>(),
                Fixture.Create<DateTime>(),
                Fixture.Create<long>(),
                Fixture.Create<long>(),
                Fixture.Create<long>(),
                "1.1");

            var startDateChange = StartDateChangeDomainModel.Get(Fixture.Create<StartDateChange>());

            if (pendingProviderApproval)
            {
                apprenticeship.AddStartDateChange(startDateChange.ActualStartDate, startDateChange.Reason,
                    null, null,
                    startDateChange.EmployerApprovedBy, startDateChange.EmployerApprovedDate, startDateChange.CreatedDate,
                    ChangeRequestStatus.Created, ChangeInitiator.Employer);
            }
            else
            {
                apprenticeship.AddStartDateChange(startDateChange.ActualStartDate, startDateChange.Reason,
                    startDateChange.ProviderApprovedBy, startDateChange.ProviderApprovedDate,
                    null, null, startDateChange.CreatedDate,
                    ChangeRequestStatus.Created, ChangeInitiator.Provider);
            }

            return apprenticeship;
        }
    }
}
