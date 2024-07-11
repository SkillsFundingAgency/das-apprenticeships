using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Events;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Domain.UnitTests.Helpers;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;
using SFA.DAS.Apprenticeships.Types;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Apprenticeship.Events;

[TestFixture]
public class WhenHandleStartDateChangeApproved
{
    private Mock<IApprenticeshipRepository> _apprenticeshipRepository;
    private Mock<IMessageSession> _messageSession;
    private StartDateChangeApprovedHandler _handler;
    private Fixture _fixture;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _fixture.Customize(new ApprenticeshipCustomization());
        _apprenticeshipRepository = new Mock<IApprenticeshipRepository>();
        _messageSession = new Mock<IMessageSession>();
        _handler = new StartDateChangeApprovedHandler(_apprenticeshipRepository.Object, _messageSession.Object);
    }

    [Test]
    public async Task ThenApprenticeshipStartDateChangedEventIsPublished()
    {
        // Arrange
        //todo fix unit test for start date change
        //var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
        //var startDateChange = _fixture.Create<StartDateChange>();
        //ApprenticeshipDomainModelTestHelper.AddEpisode(apprenticeship);
        //ApprenticeshipDomainModelTestHelper.AddPendingStartDateChange(apprenticeship, startDateChange);
        //var employerUserId = _fixture.Create<string>();
        //apprenticeship.ApproveStartDateChange(employerUserId);
        //var command = new StartDateChangeApproved(apprenticeship.Key, apprenticeship.StartDateChanges.Single().Key, ApprovedBy.Employer, _fixture<EpisodeDomainModel.AmendedPrices>());

        //_apprenticeshipRepository.Setup(x => x.Get(command.ApprenticeshipKey)).ReturnsAsync(apprenticeship);

        //// Act
        //await _handler.Handle(command);

        //// Assert
        //_messageSession.Verify(x => x.Publish(It.Is<ApprenticeshipStartDateChangedEvent>(e =>
        //    e.ApprenticeshipKey == apprenticeship.Key &&
        //    e.EmployerAccountId == apprenticeship.LatestEpisode.EmployerAccountId &&
        //    e.ApprenticeshipId == apprenticeship.ApprovalsApprenticeshipId &&
        //    e.ProviderId == apprenticeship.LatestEpisode.Ukprn &&
        //    e.ActualStartDate == startDateChange.ActualStartDate &&
        //    e.PlannedEndDate == startDateChange.PlannedEndDate &&
        //    e.AgeAtStartOfApprenticeship == apprenticeship.AgeAtStartOfApprenticeship &&
        //    e.ProviderApprovedBy == startDateChange.ProviderApprovedBy &&
        //    e.EmployerApprovedBy == employerUserId &&
        //    e.Initiator == ChangeInitiator.Provider.ToString()
        //), It.IsAny<PublishOptions>()));
    }
}