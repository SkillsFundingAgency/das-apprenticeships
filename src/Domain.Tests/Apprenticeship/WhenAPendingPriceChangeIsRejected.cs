﻿using System.Linq;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;
using SFA.DAS.Learning.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Learning.Domain.Apprenticeship;
using SFA.DAS.Learning.Enums;

namespace SFA.DAS.Learning.Domain.UnitTests.Apprenticeship;

[TestFixture]
public class WhenAPendingPriceChangeIsRejected
{
    private Fixture _fixture = null!;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
        _fixture.Customize(new ApprenticeshipCustomization());
    }

    [Test]
    public void ThenThePriceHistoryRecordIsCancelled()
    {
        // Assert
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
            priceHistoryDomainModel.ChangeReason!,
            null,
            priceHistoryDomainModel.ProviderApprovedDate,
            priceHistoryDomainModel.EmployerApprovedDate,
            priceHistoryDomainModel.Initiator);
        var reason = _fixture.Create<string>();

        // Act
        apprenticeship.RejectPendingPriceChange(reason);

        // Assert
        var priceHistory = apprenticeship.GetEntity().PriceHistories.Single(x => x.PriceChangeRequestStatus == ChangeRequestStatus.Rejected);
        priceHistory.Should().NotBeNull();
        priceHistory.RejectReason.Should().Be(reason);
    }
}