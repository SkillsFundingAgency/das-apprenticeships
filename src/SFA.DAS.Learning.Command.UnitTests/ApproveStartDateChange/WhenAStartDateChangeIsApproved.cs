using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.TestHelpers;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Learning.Command.ApproveStartDateChange;
using SFA.DAS.Learning.Domain.Apprenticeship;
using SFA.DAS.Learning.Domain.Repositories;
using SFA.DAS.Learning.Enums;
using SFA.DAS.Learning.Types;
using FundingPlatform = SFA.DAS.Learning.Enums.FundingPlatform;

namespace SFA.DAS.Apprenticeships.Command.UnitTests.ApproveStartDateChange;

[TestFixture]
public class WhenAStartDateChangeIsApproved
{
    private ApproveStartDateChangeCommandHandler _commandHandler = null!;
    private Mock<IApprenticeshipRepository> _apprenticeshipRepository = null!;
    private Mock<IMessageSession> _messageSession = null!;
    private Mock<ILogger<ApproveStartDateChangeCommandHandler>> _logger = null!;
    private Fixture _fixture = null!;

    [SetUp]
    public void SetUp()
    {
        _apprenticeshipRepository = new Mock<IApprenticeshipRepository>();
        _messageSession = new Mock<IMessageSession>();
        _logger = new Mock<ILogger<ApproveStartDateChangeCommandHandler>>();
        _commandHandler = new ApproveStartDateChangeCommandHandler(_apprenticeshipRepository.Object, _messageSession.Object, _logger.Object);

        _fixture = new Fixture();
        _fixture.Customize(new ApprenticeshipCustomization());
    }

    [Test]
    public async Task ByEmployerThenTheStartDateChangeIsApproved()
    {
        //Arrange
        var command = _fixture.Create<ApproveStartDateChangeCommand>();
        var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
        ApprenticeshipDomainModelTestHelper.AddEpisode(apprenticeship);
        var startDate = _fixture.Create<DateTime>();
        ApprenticeshipDomainModelTestHelper.AddPendingStartDateChange(apprenticeship, ChangeInitiator.Provider, startDate);
        _apprenticeshipRepository.Setup(x => x.Get(command.ApprenticeshipKey)).ReturnsAsync(apprenticeship);

        //Act
        await _commandHandler.Handle(command);

        //Assert
        _apprenticeshipRepository.Verify(x => x.Update(
            It.Is<ApprenticeshipDomainModel>(y =>
                y.GetEntity().StartDateChanges
                .Count(z => z.RequestStatus == ChangeRequestStatus.Approved
                            && z.EmployerApprovedBy == command.UserId) == 1
                && y.StartDate == startDate)));

        AssertMessageSent(startDate, apprenticeship, command.UserId);
    }

    [Test]
    public async Task ByProviderThenTheStartDateChangeIsApproved()
    {
        //Arrange
        var command = _fixture.Create<ApproveStartDateChangeCommand>();
        var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
        ApprenticeshipDomainModelTestHelper.AddEpisode(apprenticeship);
        var startDate = _fixture.Create<DateTime>();
        ApprenticeshipDomainModelTestHelper.AddPendingStartDateChange(apprenticeship, ChangeInitiator.Employer, startDate);
        _apprenticeshipRepository.Setup(x => x.Get(command.ApprenticeshipKey)).ReturnsAsync(apprenticeship);

        //Act
        await _commandHandler.Handle(command);

        //Assert
        _apprenticeshipRepository.Verify(x => x.Update(
            It.Is<ApprenticeshipDomainModel>(y =>
                y.GetEntity().StartDateChanges
                    .Count(z => z.RequestStatus == ChangeRequestStatus.Approved
                                && z.ProviderApprovedBy == command.UserId) == 1
                && y.StartDate == startDate)));
    }

    [Test]
    public async Task WhenFundingPlatformIsNotDASThenEventNotPublished()
    {
        //Arrange
        var command = _fixture.Create<ApproveStartDateChangeCommand>();
        var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
        ApprenticeshipDomainModelTestHelper.AddEpisode(apprenticeship, fundingPlatform: FundingPlatform.SLD);
        var startDate = _fixture.Create<DateTime>();
        ApprenticeshipDomainModelTestHelper.AddPendingStartDateChange(apprenticeship, ChangeInitiator.Employer, startDate);
        _apprenticeshipRepository.Setup(x => x.Get(command.ApprenticeshipKey)).ReturnsAsync(apprenticeship);

        //Act
        await _commandHandler.Handle(command);

        //Assert
        _messageSession.Verify(x => x.Publish(It.IsAny<ApprenticeshipStartDateChangedEvent>(), It.IsAny<PublishOptions>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private void AssertMessageSent(DateTime actualStartDate, ApprenticeshipDomainModel apprenticeship, string approvingUserId)
    {
        var startDateChangeDomainModel = apprenticeship.StartDateChanges.First(x => x.RequestStatus == ChangeRequestStatus.Approved);
        _messageSession.Verify(x => x.Publish(It.Is<ApprenticeshipStartDateChangedEvent>(e =>
            e.StartDate == actualStartDate &&
            IsMarkedApprovedByEmployer(e, startDateChangeDomainModel, approvingUserId) &&
            DoApprenticeshipDetailsMatchDomainModel(e, apprenticeship) &&
            ApprenticeshipDomainModelTestHelper.DoEpisodeDetailsMatchDomainModel(e, apprenticeship)
            ), It.IsAny<PublishOptions>(),
            It.IsAny<CancellationToken>()));
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