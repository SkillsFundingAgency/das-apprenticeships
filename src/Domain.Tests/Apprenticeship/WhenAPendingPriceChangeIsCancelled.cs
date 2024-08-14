﻿using System.Linq;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Apprenticeship;

[TestFixture]
public class WhenAPendingPriceChangeIsCancelled
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
        
        // Act
        apprenticeship.CancelPendingPriceChange();

        // Assert
        var priceHistory = apprenticeship.GetEntity().PriceHistories.Single(x => x.PriceChangeRequestStatus == ChangeRequestStatus.Cancelled);
        priceHistory.Should().NotBeNull();
    }
}