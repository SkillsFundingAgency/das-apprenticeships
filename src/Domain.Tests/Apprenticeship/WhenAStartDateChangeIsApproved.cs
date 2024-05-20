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
    public void ThenTheStartDateChangeRecordIsUpdated_EmployerApproval()
    {
        //Arrange
        var approverUserId = _fixture.Create<string>();
        var apprenticeship = StartDateChangeTestHelper.BuildApprenticeshipWithPendingStartDateChange();

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
        var apprenticeship = StartDateChangeTestHelper.BuildApprenticeshipWithPendingStartDateChange(pendingProviderApproval:true);

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
        var apprenticeship = StartDateChangeTestHelper.BuildApprenticeshipWithPendingStartDateChange();

        //Act
        apprenticeship.ApproveStartDateChange(employerUserId);

        var events = apprenticeship.FlushEvents();
        events.Should().ContainSingle(x => x.GetType() == typeof(StartDateChangeApproved));
    }
}