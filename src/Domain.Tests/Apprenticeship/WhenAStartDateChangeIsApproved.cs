﻿using System;
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
    public void ThenTheStartDateChangeRecordIsUpdated()
    {
        //Arrange
        var employerUserId = _fixture.Create<string>();
        var apprenticeship = BuildApprenticeshipWithPendingPriceChange();

        //Act
        apprenticeship.ApproveStartDateChange(employerUserId);

        //Assert
        apprenticeship.GetEntity().StartDateChanges.Any(x => x.RequestStatus == ChangeRequestStatus.Created).Should().BeFalse();
        var startDateChange = apprenticeship.GetEntity().StartDateChanges.Single(x => x.RequestStatus == ChangeRequestStatus.Approved);
        startDateChange.Should().NotBeNull();
        startDateChange.EmployerApprovedBy.Should().Be(employerUserId);
        startDateChange.EmployerApprovedDate.Should().NotBeNull();
    }

    [Test]
    public void ThenAStartDateChangeApprovedEventIsAdded()
    {
        //Arrange
        var employerUserId = _fixture.Create<string>();
        var apprenticeship = BuildApprenticeshipWithPendingPriceChange();

        //Act
        apprenticeship.ApproveStartDateChange(employerUserId);

        var events = apprenticeship.FlushEvents();
        events.Should().ContainSingle(x => x.GetType() == typeof(StartDateChangeApproved));
    }

    private ApprenticeshipDomainModel BuildApprenticeshipWithPendingPriceChange()
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
            _fixture.Create<long>());

        var startDateChange = StartDateChangeDomainModel.Get(_fixture.Create<StartDateChange>());

        apprenticeship.AddStartDateChange(startDateChange.ActualStartDate, startDateChange.Reason,
            startDateChange.ProviderApprovedBy, startDateChange.ProviderApprovedDate,
            null, null, startDateChange.CreatedDate,
            ChangeRequestStatus.Created, startDateChange.Initiator);

        return apprenticeship;
    }
}