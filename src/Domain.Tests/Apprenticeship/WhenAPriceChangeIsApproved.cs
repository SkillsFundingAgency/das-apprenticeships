﻿using System;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Events;
using SFA.DAS.Apprenticeships.Domain.UnitTests.Helpers;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Apprenticeship;

[TestFixture]
public class WhenAPriceChangeIsApproved
{
    private Fixture _fixture;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
        _fixture.Customize(new ApprenticeshipCustomization());
    }

    [Test]
    public void ByEmployerThenPriceHistoryRecordIsUpdatedAndEventAdded()
    {
        //Arrange
        var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
        ApprenticeshipDomainModelTestHelper.AddEpisode(apprenticeship);
        ApprenticeshipDomainModelTestHelper.AddPendingPriceChangeProviderInitiated(apprenticeship, effectiveFromDate:apprenticeship.LatestPrice.StartDate.AddDays(_fixture.Create<int>()));
        var employerUserId = _fixture.Create<string>();

        //Act
        apprenticeship.ApprovePriceChange(employerUserId, null, null);
        var events = apprenticeship.FlushEvents();

        //Assert
        apprenticeship.GetEntity().PriceHistories.Any(x => x.PriceChangeRequestStatus == ChangeRequestStatus.Created).Should().BeFalse();
        var priceHistory = apprenticeship.GetEntity().PriceHistories.Single(x => x.PriceChangeRequestStatus == ChangeRequestStatus.Approved);
        priceHistory.Should().NotBeNull();
        priceHistory.ProviderApprovedBy.Should().NotBeNull();
        priceHistory.ProviderApprovedDate.Should().NotBeNull();
        priceHistory.EmployerApprovedBy.Should().Be(employerUserId);
        priceHistory.EmployerApprovedDate.Should().NotBeNull();
        events.Should().ContainSingle(x => x.GetType() == typeof(PriceChangeApproved));
    }

    [Test]
    public void ByProviderThenPriceHistoryRecordIsUpdatedAndEventAdded()
    {
        //Arrange
        var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
        ApprenticeshipDomainModelTestHelper.AddEpisode(apprenticeship);
        var trainingPrice = _fixture.Create<int>();
        var assessmentPrice = _fixture.Create<int>();
        ApprenticeshipDomainModelTestHelper.AddPendingPriceChangeEmployerInitiated(apprenticeship, trainingPrice + assessmentPrice, effectiveFromDate:apprenticeship.LatestPrice.StartDate.AddDays(_fixture.Create<int>()));
        var providerUserId = _fixture.Create<string>();

        //Act
        apprenticeship.ApprovePriceChange(providerUserId, trainingPrice, assessmentPrice);
        var events = apprenticeship.FlushEvents();

        //Assert
        apprenticeship.GetEntity().PriceHistories.Any(x => x.PriceChangeRequestStatus == ChangeRequestStatus.Created).Should().BeFalse();
        var priceHistory = apprenticeship.GetEntity().PriceHistories.Single(x => x.PriceChangeRequestStatus == ChangeRequestStatus.Approved);
        priceHistory.Should().NotBeNull();
        priceHistory.ProviderApprovedBy.Should().Be(providerUserId);
        priceHistory.ProviderApprovedDate.Should().NotBeNull();
        priceHistory.EmployerApprovedBy.Should().NotBeNull();
        priceHistory.EmployerApprovedDate.Should().NotBeNull();
        events.Should().ContainSingle(x => x.GetType() == typeof(PriceChangeApproved));
    }

    [Test]
    public void AndTheEffectiveDateIsBeforeTheLatestEpisode_ThenTheLatestEpisodeIsDeleted()
    {
        //Arrange
        var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
        var originalStartDate = new DateTime(2024,1,1);
        var originalEndDate = new DateTime(2024, 10, 1);
        ApprenticeshipDomainModelTestHelper.AddEpisode(apprenticeship, originalStartDate, originalEndDate);

        CreatePriceChange(apprenticeship, originalEndDate.AddDays(-5), _fixture.Create<int>(), _fixture.Create<int>());


        //Act
        CreatePriceChange(apprenticeship, originalEndDate.AddDays(-10), _fixture.Create<int>(), _fixture.Create<int>());

        //Assert
        var apprenticeshipEntity = apprenticeship.GetEntity();
        apprenticeshipEntity.PriceHistories.Should().HaveCount(2);
        var prices = apprenticeshipEntity.Episodes.Single().Prices;
        prices.Should().HaveCount(3);
        prices.Count(x => x.IsDeleted).Should().Be(1);
        prices.Count(x => x.StartDate == originalStartDate).Should().Be(1);
        prices.Count(x => x.EndDate == originalEndDate && !x.IsDeleted).Should().Be(1);
    }


    [Test]
    public void AndTheEffectiveDateIsBeforeTheLatestEpisodeButNotEarlierEpisode_ThenTheLatestEpisodeIsDeleted()
    {
        //Arrange
        var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
        var originalStartDate = new DateTime(2024, 1, 1);
        var originalEndDate = new DateTime(2024, 10, 1);
        var firstPriceChangeDate = new DateTime(2024, 6, 1);
        var secondPriceChangeDate = new DateTime(2024, 7, 25);
        var finalPriceChangeDate = new DateTime(2024, 6, 15);

        ApprenticeshipDomainModelTestHelper.AddEpisode(apprenticeship, originalStartDate, originalEndDate);

        CreatePriceChange(apprenticeship, firstPriceChangeDate, _fixture.Create<int>(), _fixture.Create<int>());
        CreatePriceChange(apprenticeship, secondPriceChangeDate, _fixture.Create<int>(), _fixture.Create<int>());

        //Act
        CreatePriceChange(apprenticeship, finalPriceChangeDate, _fixture.Create<int>(), _fixture.Create<int>());

        //Assert
        var apprenticeshipEntity = apprenticeship.GetEntity();
        apprenticeshipEntity.PriceHistories.Should().HaveCount(3);
        var prices = apprenticeshipEntity.Episodes.Single().Prices;
        prices.Should().HaveCount(4);
        prices.Count(x => x.IsDeleted).Should().Be(1);
        prices.Count(x => x.StartDate == originalStartDate && x.EndDate == firstPriceChangeDate.AddDays(-1) && !x.IsDeleted).Should().Be(1);
        prices.Count(x => x.StartDate == firstPriceChangeDate && x.EndDate == finalPriceChangeDate.AddDays(-1) && !x.IsDeleted).Should().Be(1);
        prices.Count(x => x.StartDate == secondPriceChangeDate && x.EndDate == originalEndDate && x.IsDeleted).Should().Be(1);
        prices.Count(x => x.StartDate == finalPriceChangeDate && x.EndDate == originalEndDate && !x.IsDeleted).Should().Be(1);
    }
    private void CreatePriceChange(ApprenticeshipDomainModel apprenticeship, DateTime effectiveFromDate, int newTrainingPrice, int newAssessmentPrice)
    {
        ApprenticeshipDomainModelTestHelper.AddPendingPriceChangeEmployerInitiated(apprenticeship, newTrainingPrice + newAssessmentPrice, effectiveFromDate);
        apprenticeship.ApprovePriceChange(_fixture.Create<string>(), newTrainingPrice, newAssessmentPrice);
        var events = apprenticeship.FlushEvents();
    }
}