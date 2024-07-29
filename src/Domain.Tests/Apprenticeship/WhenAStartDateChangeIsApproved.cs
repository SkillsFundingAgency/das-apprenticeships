using System;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Events;
using SFA.DAS.Apprenticeships.Domain.UnitTests.Helpers;
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
    public void ByProviderThenTheStartDateChangeRecordIsUpdated()
    {
        //Arrange
        var approverUserId = _fixture.Create<string>();
        var apprenticeship = ApprenticeshipDomainModelTestHelper.BuildApprenticeshipWithPendingStartDateChange(pendingProviderApproval: true);

        //Act
        var now = DateTime.UtcNow;
        apprenticeship.ApproveStartDateChange(approverUserId, _fixture.Create<int>());

        //Assert
        var entity = apprenticeship.GetEntity();
        entity.StartDateChanges.Any(x => x.RequestStatus == ChangeRequestStatus.Created).Should().BeFalse();
        var startDateChange = entity.StartDateChanges.Single(x => x.RequestStatus == ChangeRequestStatus.Approved);
        startDateChange.Should().NotBeNull();
        startDateChange.ProviderApprovedBy.Should().Be(approverUserId);
        startDateChange.ProviderApprovedDate.Should().BeAfter(now);
        startDateChange.EmployerApprovedBy.Should().NotBeNull();
        startDateChange.EmployerApprovedDate.Should().NotBeNull();
    }

    [Test]
    public void ByEmployerThenTheStartDateChangeRecordIsUpdated()
    {
        //Arrange
        var approverUserId = _fixture.Create<string>();
        var apprenticeship = ApprenticeshipDomainModelTestHelper.BuildApprenticeshipWithPendingStartDateChange(pendingProviderApproval: false);

        //Act
        var now = DateTime.UtcNow;
        apprenticeship.ApproveStartDateChange(approverUserId, _fixture.Create<int>());

        //Assert
        var entity = apprenticeship.GetEntity();
        entity.StartDateChanges.Any(x => x.RequestStatus == ChangeRequestStatus.Created).Should().BeFalse();
        var startDateChange = entity.StartDateChanges.Single(x => x.RequestStatus == ChangeRequestStatus.Approved);
        startDateChange.Should().NotBeNull();
        startDateChange.EmployerApprovedBy.Should().Be(approverUserId);
        startDateChange.EmployerApprovedDate.Should().BeAfter(now);
        startDateChange.ProviderApprovedBy.Should().NotBeNull();
        startDateChange.ProviderApprovedDate.Should().NotBeNull();
    }

    //TODO start date change - Add unit tests for the correct handing of episodes and prices

    [Test]
    public void ThenAStartDateChangeApprovedEventIsAdded()
    {
        //Arrange
        var employerUserId = _fixture.Create<string>();
        var apprenticeship = ApprenticeshipDomainModelTestHelper.BuildApprenticeshipWithPendingStartDateChange();

        //Act
        apprenticeship.ApproveStartDateChange(employerUserId, _fixture.Create<int>());

        //Assert
        var events = apprenticeship.FlushEvents();
        events.Should().ContainSingle(x => x.GetType() == typeof(StartDateChangeApproved));
    }
}