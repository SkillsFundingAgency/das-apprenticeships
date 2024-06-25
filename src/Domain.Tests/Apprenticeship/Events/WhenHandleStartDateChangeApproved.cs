using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Events;
using SFA.DAS.Apprenticeships.Domain.Factories;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Enums;
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
        _apprenticeshipRepository = new Mock<IApprenticeshipRepository>();
        _messageSession = new Mock<IMessageSession>();
        _handler = new StartDateChangeApprovedHandler(_apprenticeshipRepository.Object, _messageSession.Object);
    }

    [Test]
    public async Task ThenApprenticeshipStartDateChangedEventIsPublished()
    {
        // Arrange
        var apprenticeshipFactory = new ApprenticeshipFactory();
        var apprenticeship = apprenticeshipFactory.CreateNew(
            "1234435",
            "TRN",
            new DateTime(2000, 10, 16),
            "Ron",
            "Swanson",
            _fixture.Create<decimal?>(),
            _fixture.Create<decimal?>(),
            _fixture.Create<decimal>(),
            _fixture.Create<string>(),
            _fixture.Create<int>(),
            _fixture.Create<DateTime>(),
            _fixture.Create<DateTime>(),
            _fixture.Create<long>(),
            _fixture.Create<long>(),
            _fixture.Create<long>(),
            "1.1");

        var startDateChange = _fixture.Create<StartDateChange>();

        apprenticeship.AddApproval(_fixture.Create<long>(), _fixture.Create<string>(), _fixture.Create<DateTime>(), _fixture.Create<DateTime>(), _fixture.Create<decimal>(), _fixture.Create<long>(), _fixture.Create<Enums.FundingType>(), _fixture.Create<int>(), _fixture.Create<DateTime?>(), _fixture.Create<Enums.FundingPlatform?>());
        apprenticeship.AddStartDateChange(startDateChange.ActualStartDate, startDateChange.PlannedEndDate, startDateChange.Reason, startDateChange.ProviderApprovedBy, startDateChange.ProviderApprovedDate, null, null, startDateChange.CreatedDate, startDateChange.RequestStatus, ChangeInitiator.Provider);
        var employerUserId = _fixture.Create<string>();
        apprenticeship.ApproveStartDateChange(employerUserId);
        var approval = apprenticeship.Episodes.Single();
        var command = new StartDateChangeApproved(apprenticeship.Key, apprenticeship.StartDateChanges.Single().Key, ApprovedBy.Employer);

        _apprenticeshipRepository.Setup(x => x.Get(command.ApprenticeshipKey)).ReturnsAsync(apprenticeship);

        // Act
        await _handler.Handle(command);

        // Assert
        _messageSession.Verify(x => x.Publish(It.Is<ApprenticeshipStartDateChangedEvent>(e =>
            e.ApprenticeshipKey == apprenticeship.Key &&
            e.EmployerAccountId == apprenticeship.EmployerAccountId &&
            e.ApprenticeshipId == approval.ApprovalsApprenticeshipId &&
            e.ProviderId == apprenticeship.Ukprn &&
            e.ActualStartDate == startDateChange.ActualStartDate &&
            e.PlannedEndDate == startDateChange.PlannedEndDate &&
            e.AgeAtStartOfApprenticeship == apprenticeship.AgeAtStartOfApprenticeship &&
            e.ProviderApprovedBy == startDateChange.ProviderApprovedBy &&
            e.EmployerApprovedBy == employerUserId &&
            e.Initiator == ChangeInitiator.Provider.ToString()
        ), It.IsAny<PublishOptions>()));
    }
}