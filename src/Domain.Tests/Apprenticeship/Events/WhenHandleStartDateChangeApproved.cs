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
    public async Task ByEmployerThenApprenticeshipStartDateChangedEventIsPublished()
    {
        // Arrange
        var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
        var startDateChange = _fixture.Create<StartDateChange>();
        ApprenticeshipDomainModelTestHelper.AddEpisode(apprenticeship);
        ApprenticeshipDomainModelTestHelper.AddPendingStartDateChangeProviderInitiated(apprenticeship, startDateChange);
        var approvingUserId = _fixture.Create<string>();
        apprenticeship.ApproveStartDateChange(approvingUserId);
        var startDateChangeDomainModel = apprenticeship.StartDateChanges.First(x => x.RequestStatus == ChangeRequestStatus.Approved);
        var command = new StartDateChangeApproved(apprenticeship.Key, apprenticeship.StartDateChanges.Single().Key, ApprovedBy.Employer);

        _apprenticeshipRepository.Setup(x => x.Get(command.ApprenticeshipKey)).ReturnsAsync(apprenticeship);

        // Act
        await _handler.Handle(command);

        // Assert
        _messageSession.Verify(x => x.Publish(It.Is<ApprenticeshipStartDateChangedEvent>(e =>
            e.StartDate == startDateChange.ActualStartDate &&
            IsMarkedApprovedByEmployer(e, startDateChangeDomainModel, approvingUserId) &&
            DoApprenticeshipDetailsMatchDomainModel(e, apprenticeship) && 
            ApprenticeshipDomainModelTestHelper.DoEpisodeDetailsMatchDomainModel(e, apprenticeship)), It.IsAny<PublishOptions>()));
    }

    [Test]
    public async Task ByProviderThenApprenticeshipStartDateChangedEventIsPublished()
    {
        // Arrange
        var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
        var startDateChange = _fixture.Create<StartDateChange>();
        ApprenticeshipDomainModelTestHelper.AddEpisode(apprenticeship);
        ApprenticeshipDomainModelTestHelper.AddPendingStartDateChangeEmployerInitiated(apprenticeship, startDateChange);
        var approvingUserId = _fixture.Create<string>();
        apprenticeship.ApproveStartDateChange(approvingUserId);
        var startDateChangeDomainModel = apprenticeship.StartDateChanges.First(x => x.RequestStatus == ChangeRequestStatus.Approved);
        var command = new StartDateChangeApproved(apprenticeship.Key, apprenticeship.StartDateChanges.Single().Key, ApprovedBy.Provider);

        _apprenticeshipRepository.Setup(x => x.Get(command.ApprenticeshipKey)).ReturnsAsync(apprenticeship);

        // Act
        await _handler.Handle(command);

        // Assert
        _messageSession.Verify(x => x.Publish(It.Is<ApprenticeshipStartDateChangedEvent>(e =>
            e.StartDate == startDateChange.ActualStartDate &&
            IsMarkedApprovedByProvider(e, startDateChangeDomainModel, approvingUserId) &&
            DoApprenticeshipDetailsMatchDomainModel(e, apprenticeship) && 
            ApprenticeshipDomainModelTestHelper.DoEpisodeDetailsMatchDomainModel(e, apprenticeship)), It.IsAny<PublishOptions>()));
    }

    private static bool IsMarkedApprovedByEmployer(ApprenticeshipStartDateChangedEvent e, StartDateChangeDomainModel startDateChange, string approvingUserId)
    {
        return
            e.ApprovedDate == startDateChange.EmployerApprovedDate!.Value &&
            e.ProviderApprovedBy == startDateChange.ProviderApprovedBy &&
            e.EmployerApprovedBy == approvingUserId &&
            e.Initiator == ChangeInitiator.Provider.ToString();
    }

    private static bool IsMarkedApprovedByProvider(ApprenticeshipStartDateChangedEvent e, StartDateChangeDomainModel startDateChange, string approvingUserId)
    {
        return
            e.ApprovedDate == startDateChange.ProviderApprovedDate!.Value &&
            e.ProviderApprovedBy == approvingUserId &&
            e.EmployerApprovedBy == startDateChange.EmployerApprovedBy &&
            e.Initiator == ChangeInitiator.Employer.ToString();
    }
    
    private static bool DoApprenticeshipDetailsMatchDomainModel(ApprenticeshipStartDateChangedEvent e, ApprenticeshipDomainModel apprenticeship)
    {
        return
            e.ApprenticeshipKey == apprenticeship.Key &&
            e.ApprenticeshipId == apprenticeship.ApprovalsApprenticeshipId;
    }
}