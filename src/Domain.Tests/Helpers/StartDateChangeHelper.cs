using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Enums;
using AutoFixture;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Helpers
{
    public static class StartDateChangeTestHelper
    {
        private static readonly Fixture Fixture = new();

        public static ApprenticeshipDomainModel BuildApprenticeshipWithPendingStartDateChange(bool pendingProviderApproval = false)
        {
            Fixture.Customize(new ApprenticeshipCustomization());
            var apprenticeship = Fixture.Create<ApprenticeshipDomainModel>();

            var startDateChange = StartDateChangeDomainModel.Get(Fixture.Create<StartDateChange>());

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
    }
}
