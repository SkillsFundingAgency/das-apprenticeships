using System;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Factories;
using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Apprenticeship;

[TestFixture]
public class WhenAPendingPriceChangeIsCancelled
{
    private ApprenticeshipDomainModel _apprenticeship = null!;
    private Fixture _fixture = null!;

    [SetUp]
    public void SetUp()
    {
        var apprenticeshipFactory = new ApprenticeshipFactory();
        _fixture = new Fixture();
        _apprenticeship = apprenticeshipFactory.CreateNew(
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
            _fixture.Create<long>());

        var priceHistory = PriceHistoryDomainModel.Get(_fixture.Create<PriceHistory>());
        _apprenticeship.AddPriceHistory(
            priceHistory.TrainingPrice,
            priceHistory.AssessmentPrice,
            priceHistory.TotalPrice,
            priceHistory.EffectiveFromDate,
            priceHistory.CreatedDate,
            priceHistory.PriceChangeRequestStatus,
            priceHistory.ProviderApprovedBy,
            priceHistory.ChangeReason);
    }

    [Test]
    public void ThenThePriceHistoryRecordIsCancelled()
    {
        _apprenticeship.CancelPendingPriceChange();

        var priceHistory = _apprenticeship.GetEntity().PriceHistories.Single(x => x.PriceChangeRequestStatus == PriceChangeRequestStatus.Cancelled);

        priceHistory.Should().NotBeNull();
    }
}