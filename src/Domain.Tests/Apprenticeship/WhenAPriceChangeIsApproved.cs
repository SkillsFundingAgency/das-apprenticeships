using System;
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
public class WhenAPriceChangeIsApproved
{
    private ApprenticeshipDomainModel _apprenticeship;
    private Fixture _fixture;

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
            _fixture.Create<long>(),
            _fixture.Create<long>(),
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
            priceHistory.ChangeReason,
            priceHistory.EmployerApprovedBy,
            priceHistory.ProviderApprovedDate,
            priceHistory.EmployerApprovedDate,
            priceHistory.Initiator);
    }

    [Test]
    public void ThenThePriceHistoryRecordIsUpdated()
    {
        //Arrange
        var employerUserId = _fixture.Create<string>();
        
        //Act
        _apprenticeship.ApprovePriceChange(employerUserId, null, null);

        //Assert
        _apprenticeship.GetEntity().PriceHistories.Any(x => x.PriceChangeRequestStatus == PriceChangeRequestStatus.Created).Should().BeFalse();
        var priceHistory = _apprenticeship.GetEntity().PriceHistories.Single(x => x.PriceChangeRequestStatus == PriceChangeRequestStatus.Approved);
        priceHistory.Should().NotBeNull();
        priceHistory.EmployerApprovedBy.Should().Be(employerUserId);
        priceHistory.EmployerApprovedDate.Should().NotBeNull();
    }

    [Test]
    public void ThenAPriceChangeApprovedEventIsAdded()
    {
        //Arrange
        var employerUserId = _fixture.Create<string>();

        //Act
        _apprenticeship.ApprovePriceChange(employerUserId, null, null);

        var events = _apprenticeship.FlushEvents();
        events.Should().ContainSingle(x => x.GetType() == typeof(PriceChangeApproved));
    }
}