﻿using System.Linq;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.TestHelpers;
using SFA.DAS.Learning.Enums;

namespace SFA.DAS.Learning.Domain.UnitTests.Apprenticeship;

[TestFixture]
public class WhenAStartDateChangeIsRejected
{
    private Fixture _fixture;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
    }

    [Test]
    public void ThenTheStartDateChangeRecordIsUpdated()
    {
        //Arrange
        var rejectReason = _fixture.Create<string>();
        var apprenticeship = ApprenticeshipDomainModelTestHelper.BuildApprenticeshipWithPendingStartDateChange();

        //Act
        apprenticeship.RejectStartDateChange(rejectReason);

        //Assert
        var entity = apprenticeship.GetEntity();
        entity.StartDateChanges.Any(x => x.RequestStatus == ChangeRequestStatus.Created).Should().BeFalse();
        entity.StartDateChanges.Should().Contain(x => x.RequestStatus == ChangeRequestStatus.Rejected && x.RejectReason == rejectReason);
    }
}