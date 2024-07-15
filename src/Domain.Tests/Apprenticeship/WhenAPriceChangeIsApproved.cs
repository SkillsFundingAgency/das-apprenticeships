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

    //TODO price change - Add unit tests for the correct handing of episodes and prices
}