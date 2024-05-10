using System;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Events;
using SFA.DAS.Apprenticeships.Domain.Factories;
using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Apprenticeship;

[TestFixture]
public class WhenAStartDateChangeIsApproved
{
    private Fixture _fixture;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
    }

    [Test]
    public void ThenTheStartDateChangeRecordIsUpdated_EmployerApproval()
    {
        //Arrange
        var approverUserId = _fixture.Create<string>();
        var apprenticeship = BuildApprenticeshipWithPendingStartDateChange();

        //Act
        apprenticeship.ApproveStartDateChange(approverUserId);

        //Assert
        var entity = apprenticeship.GetEntity();
        entity.StartDateChanges.Any(x => x.RequestStatus == ChangeRequestStatus.Created).Should().BeFalse();
        var startDateChange = entity.StartDateChanges.Single(x => x.RequestStatus == ChangeRequestStatus.Approved);
        startDateChange.Should().NotBeNull();
        startDateChange.EmployerApprovedBy.Should().Be(approverUserId);
        startDateChange.EmployerApprovedDate.Should().NotBeNull();
    }

    [Test]
    public void ThenTheStartDateChangeRecordIsUpdated_ProviderApproval()
    {
        //Arrange
        var approverUserId = _fixture.Create<string>();
        var apprenticeship = BuildApprenticeshipWithPendingStartDateChange(pendingProviderApproval:true);

        //Act
        apprenticeship.ApproveStartDateChange(approverUserId);

        //Assert
        var entity = apprenticeship.GetEntity();
        entity.StartDateChanges.Any(x => x.RequestStatus == ChangeRequestStatus.Created).Should().BeFalse();
        var startDateChange = entity.StartDateChanges.Single(x => x.RequestStatus == ChangeRequestStatus.Approved);
        startDateChange.Should().NotBeNull();
        startDateChange.ProviderApprovedBy.Should().Be(approverUserId);
        startDateChange.ProviderApprovedDate.Should().NotBeNull();
    }

    [Test]
    public void ThenAStartDateChangeApprovedEventIsAdded()
    {
        //Arrange
        var employerUserId = _fixture.Create<string>();
        var apprenticeship = BuildApprenticeshipWithPendingStartDateChange();

        //Act
        apprenticeship.ApproveStartDateChange(employerUserId);

        var events = apprenticeship.FlushEvents();
        events.Should().ContainSingle(x => x.GetType() == typeof(StartDateChangeApproved));
    }

    private ApprenticeshipDomainModel BuildApprenticeshipWithPendingStartDateChange(bool pendingProviderApproval = false)
    {
        var apprenticeship = new ApprenticeshipFactory().CreateNew(
            "1234435",
            "TRN",
            new DateTime(2000,
                10,
                16),
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

        var startDateChange = StartDateChangeDomainModel.Get(_fixture.Create<StartDateChange>());

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