﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.DataAccess;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.TestHelpers;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Repositories.ApprenticeshipQueryRepository
{
    public class WhenGettingPriceHistory
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
        public async Task ThenTheCorrectPendingPriceChangeIsReturned()
        {
            //Arrange
            SetUpApprenticeshipQueryRepository();
            
            //Act
            var apprenticeshipKey = _fixture.Create<Guid>();
            var otherApprenticeshipKey = _fixture.Create<Guid>();
            var priceHistoryKey = _fixture.Create<Guid>();
            var effectiveFromDate = DateTime.UtcNow.AddDays(-5).Date;
            
            var apprenticeships = new[]
            {
                _fixture.Build<DataAccess.Entities.Apprenticeship.Apprenticeship>()
                    .With(x => x.Key, apprenticeshipKey)
                    .With(x => x.PriceHistories, new List<PriceHistory>() { new()
                        {
                            Key = priceHistoryKey,
                            ApprenticeshipKey = apprenticeshipKey,
                            PriceChangeRequestStatus = ChangeRequestStatus.Created,
                            TrainingPrice = 10000,
                            AssessmentPrice = 3000,
                            TotalPrice = 13000,
                            EffectiveFromDate = effectiveFromDate,
                            ChangeReason = "testReason"
                        }
                    })
                    .Create(), 
                _fixture.Build<DataAccess.Entities.Apprenticeship.Apprenticeship>()
                    .With(x => x.Key, otherApprenticeshipKey)
                    .With(x => x.PriceHistories, new List<PriceHistory>() { new() { ApprenticeshipKey = otherApprenticeshipKey}})
                    .Create()
            };

            await _dbContext.AddRangeAsync(apprenticeships);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _sut.GetPendingPriceChange(apprenticeshipKey);

            // Assert
            result.Should().NotBeNull();
            result.OriginalTrainingPrice = apprenticeships[0].TrainingPrice;
            result.OriginalAssessmentPrice = apprenticeships[0].EndPointAssessmentPrice;
            result.OriginalTotalPrice = apprenticeships[0].TotalPrice;
            result.PendingTrainingPrice = 10000;
            result.PendingAssessmentPrice = 3000;
            result.PendingTotalPrice = 13000;
            result.EffectiveFrom = effectiveFromDate;
            result.Reason = "testReason";
            result.Ukprn = apprenticeships[0].Ukprn;
        }
        
        private void SetUpApprenticeshipQueryRepository()
        {
            _dbContext = InMemoryDbContextCreator.SetUpInMemoryDbContext();
            var logger = Mock.Of<ILogger<Domain.Repositories.ApprenticeshipQueryRepository>>();
            _sut = new Domain.Repositories.ApprenticeshipQueryRepository(new Lazy<ApprenticeshipsDataContext>(_dbContext), logger);
        }
    }
}
