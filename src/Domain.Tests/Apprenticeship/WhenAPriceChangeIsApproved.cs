using System.Linq;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
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
    public void ThenPriceHistoryRecordIsUpdatedAndEventAdded()
    {
        //Arrange
        var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
        var priceHistoryDomainModel = PriceHistoryDomainModel.Get(_fixture.Create<PriceHistory>());
        apprenticeship.AddPriceHistory(
            priceHistoryDomainModel.TrainingPrice,
            priceHistoryDomainModel.AssessmentPrice,
            priceHistoryDomainModel.TotalPrice,
            priceHistoryDomainModel.EffectiveFromDate,
            priceHistoryDomainModel.CreatedDate,
            priceHistoryDomainModel.PriceChangeRequestStatus,
            priceHistoryDomainModel.ProviderApprovedBy,
            priceHistoryDomainModel.ChangeReason,
            priceHistoryDomainModel.EmployerApprovedBy,
            priceHistoryDomainModel.ProviderApprovedDate,
            priceHistoryDomainModel.EmployerApprovedDate,
            priceHistoryDomainModel.Initiator);
        var employerUserId = _fixture.Create<string>();
        ApprenticeshipDomainModelTestHelper.AddEpisode(apprenticeship);

        //Act
        apprenticeship.ApprovePriceChange(employerUserId, null, null);
        var events = apprenticeship.FlushEvents();

        //Assert
        apprenticeship.GetEntity().PriceHistories.Any(x => x.PriceChangeRequestStatus == ChangeRequestStatus.Created).Should().BeFalse();
        var priceHistory = apprenticeship.GetEntity().PriceHistories.Single(x => x.PriceChangeRequestStatus == ChangeRequestStatus.Approved);
        priceHistory.Should().NotBeNull();
        priceHistory.EmployerApprovedBy.Should().Be(employerUserId);
        priceHistory.EmployerApprovedDate.Should().NotBeNull();
        events.Should().ContainSingle(x => x.GetType() == typeof(PriceChangeApproved));
    }

    //TODO price change - Add unit tests for the correct handing of episodes and prices
}