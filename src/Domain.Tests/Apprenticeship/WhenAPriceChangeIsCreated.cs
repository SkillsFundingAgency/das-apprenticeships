using System;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Models;
using SFA.DAS.Apprenticeships.Domain.Factories;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Apprenticeship;

[TestFixture]
public class WhenAPriceChangeIsCreated
{
    private Domain.Apprenticeship.ApprenticeshipDomainModel _apprenticeship;
    private Fixture _fixture;

    [SetUp]
    public void SetUp()
    {
        var apprenticeshipFactory = new ApprenticeshipFactory();
        _fixture = new Fixture();
        _apprenticeship = apprenticeshipFactory.CreateNew("1234435", "TRN", new DateTime(2000, 10, 16), "Ron", "Swanson", _fixture.Create<decimal?>(), _fixture.Create<decimal?>(), _fixture.Create<decimal>(), _fixture.Create<string>(), _fixture.Create<int>());
    }

    [Test]
    public void ThenThePriceHistoryRecordIsAdded()
    {
        var expectedModel = _fixture.Create<PriceHistoryModel>();
        _apprenticeship.AddPriceHistory(
            expectedModel.TrainingPrice,
            expectedModel.AssessmentPrice,
            expectedModel.TotalPrice,
            expectedModel.EffectiveFromDate,
            expectedModel.CreatedDate,
            expectedModel.PriceChangeRequestStatus);

        var priceHistory = _apprenticeship.GetEntity().PriceHistories.Single(x => x.EffectiveFromDate == expectedModel.EffectiveFromDate);

        priceHistory.Should()
            .BeEquivalentTo(expectedModel,
                opts => opts.Excluding(x => x.Key)
                    .Excluding(x => x.ApprenticeshipKey)
                    .Excluding(x => x.ApprenticeshipId)
                    .Excluding(x => x.ProviderApprovedBy)
                    .Excluding(x => x.ProviderApprovedDate)
                    .Excluding(x => x.EmployerApprovedBy)
                    .Excluding(x => x.EmployerApprovedDate));
    }
}