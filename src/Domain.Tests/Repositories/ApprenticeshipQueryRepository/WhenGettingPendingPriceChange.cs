using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.DataAccess;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.UnitTests.Helpers;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.TestHelpers;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Repositories.ApprenticeshipQueryRepository
{
    public class WhenGettingPendingPriceChange
    {
        private Domain.Repositories.ApprenticeshipQueryRepository _sut;
        private Fixture _fixture;
        private ApprenticeshipsDataContext _dbContext;

        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();
        }

        [TearDown]
        public void CleanUp()
        {
            _dbContext.Dispose();
        }

        [Test]
        public async Task ThenReturnNullWhenNoApprenticeshipFoundWithApprenticeshipKey()
        {
            //Arrange
            SetUpApprenticeshipQueryRepository();

            //Act
            var result = await _sut.GetPendingPriceChange(_fixture.Create<Guid>());

            //Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task ThenReturnNullWhenNoPriceHistoryRecordsFoundForExistingApprenticeship()
        {
            //Arrange
            SetUpApprenticeshipQueryRepository();
            var apprenticeshipKey = _fixture.Create<Guid>();
            await _dbContext.AddApprenticeship(_fixture.Create<Guid>(), true, initiator: "Provider");

            // Act
            var result = await _sut.GetPendingPriceChange(apprenticeshipKey);

            // Assert
            result.Should().BeNull();
        }

        [TestCase("Employer")]
        [TestCase("Provider")]
        public async Task ThenTheCorrectPendingPriceChangeIsReturned(string initiator)
        {
            //Arrange
            SetUpApprenticeshipQueryRepository();

            var apprenticeshipKey = _fixture.Create<Guid>();
            await _dbContext.AddApprenticeship(apprenticeshipKey, true, initiator: initiator);
            var episodePrice = _dbContext.EpisodePrices.Single();
            var pendingRequest = _dbContext.PriceHistories.Single();
            var episode = _dbContext.Episodes.Single();
            await _dbContext.AddApprenticeship(_fixture.Create<Guid>(), true, initiator: initiator);

            // Act
            var result = await _sut.GetPendingPriceChange(apprenticeshipKey);

            // Assert
            result.Should().NotBeNull();
            result.OriginalTrainingPrice.Should().Be(episodePrice.TrainingPrice);
            result.OriginalAssessmentPrice.Should().Be(episodePrice.EndPointAssessmentPrice);
            result.OriginalTotalPrice.Should().Be(episodePrice.TotalPrice);
            result.PendingTrainingPrice.Should().Be(10000);
            result.PendingAssessmentPrice.Should().Be(3000);
            result.PendingTotalPrice.Should().Be(13000);
            result.EffectiveFrom.Should().Be(pendingRequest.EffectiveFromDate);
            result.Reason.Should().Be("testReason");
            result.Ukprn.Should().Be(episode.Ukprn);
            result.ProviderApprovedDate.Should().Be(pendingRequest.ProviderApprovedDate);
            result.EmployerApprovedDate.Should().Be(pendingRequest.EmployerApprovedDate);
            result.AccountLegalEntityId.Should().Be(episode.AccountLegalEntityId);
            result.Initiator.Should().Be(initiator);
        }

        private void SetUpApprenticeshipQueryRepository()
        {
            _dbContext = InMemoryDbContextCreator.SetUpInMemoryDbContext();
            var logger = Mock.Of<ILogger<Domain.Repositories.ApprenticeshipQueryRepository>>();
            _sut = new Domain.Repositories.ApprenticeshipQueryRepository(new Lazy<ApprenticeshipsDataContext>(_dbContext), logger);
        }
    }
}
