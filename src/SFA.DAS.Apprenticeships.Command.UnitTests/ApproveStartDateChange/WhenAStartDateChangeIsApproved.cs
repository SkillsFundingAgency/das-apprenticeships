﻿using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Command.ApproveStartDateChange;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.Infrastructure.Services;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;

namespace SFA.DAS.Apprenticeships.Command.UnitTests.ApproveStartDateChange;

[TestFixture]
public class WhenAStartDateChangeIsApproved
{
    private ApproveStartDateChangeCommandHandler _commandHandler = null!;
    private Mock<IApprenticeshipRepository> _apprenticeshipRepository = null!;
    private Fixture _fixture = null!;

    [SetUp]
    public void SetUp()
    {
        _apprenticeshipRepository = new Mock<IApprenticeshipRepository>();
        _commandHandler = new ApproveStartDateChangeCommandHandler(_apprenticeshipRepository.Object);

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
}