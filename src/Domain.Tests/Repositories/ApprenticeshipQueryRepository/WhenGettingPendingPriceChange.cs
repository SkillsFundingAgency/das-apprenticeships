﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.DataAccess;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Enums;

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
            var logger = Mock.Of<ILogger<Domain.Repositories.ApprenticeshipQueryRepository>>();

            var options = new DbContextOptionsBuilder<ApprenticeshipsDataContext>().UseInMemoryDatabase("ApprenticeshipsDbContext" + Guid.NewGuid()).Options;
            _dbContext = new ApprenticeshipsDataContext(options);

            _sut = new Domain.Repositories.ApprenticeshipQueryRepository(new Lazy<ApprenticeshipsDataContext>(_dbContext), logger);
        }

        [TearDown]
        public void CleanUp()
        {
            _dbContext.Dispose();
        }

        [Test]
        public async Task ThenReturnNullWhenNoApprenticeshipFoundWithApprenticeshipKey()
        {
            //Act
            var result = await _sut.GetPendingPriceChange(_fixture.Create<Guid>());

            //Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task ThenReturnNullWhenNoPriceHistoryRecordsFoundForExistingApprenticeship()
        {
            //Act
            var apprenticeshipKey = _fixture.Create<Guid>();
            var otherApprenticeshipKey1 = _fixture.Create<Guid>();
            var otherApprenticeshipKey2 = _fixture.Create<Guid>();
            
            var apprenticeships = new[]
            {
                _fixture.Build<DataAccess.Entities.Apprenticeship.Apprenticeship>()
                    .With(x => x.Key, otherApprenticeshipKey1)
                    .With(x => x.PriceHistories, new List<PriceHistory>() { new() { ApprenticeshipKey = otherApprenticeshipKey1}})
                    .Create(), 
                _fixture.Build<DataAccess.Entities.Apprenticeship.Apprenticeship>()
                    .With(x => x.Key, otherApprenticeshipKey2)
                    .With(x => x.PriceHistories, new List<PriceHistory>() { new() { ApprenticeshipKey = otherApprenticeshipKey2}})
                    .Create()
            };

            await _dbContext.AddRangeAsync(apprenticeships);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _sut.GetPendingPriceChange(apprenticeshipKey);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task ThenTheCorrectPendingPriceChangeIsReturned()
        {
            //Act
            var apprenticeshipKey = _fixture.Create<Guid>();
            var otherApprenticeshipKey = _fixture.Create<Guid>();
            var priceHistoryKey = _fixture.Create<Guid>();
            var effectiveFromDate = DateTime.UtcNow.AddDays(-5).Date;
            var providerApprovedDate = _fixture.Create<DateTime?>();
            var employerApprovedDate = _fixture.Create<DateTime?>();

            var apprenticeships = new[]
            {
                _fixture.Build<DataAccess.Entities.Apprenticeship.Apprenticeship>()
                    .With(x => x.Key, apprenticeshipKey)
                    .With(x => x.PriceHistories, new List<PriceHistory>() { new()
                        {
                            Key = priceHistoryKey,
                            ApprenticeshipKey = apprenticeshipKey,
                            PriceChangeRequestStatus = PriceChangeRequestStatus.Created,
                            TrainingPrice = 10000,
                            AssessmentPrice = 3000,
                            TotalPrice = 13000,
                            EffectiveFromDate = effectiveFromDate,
                            ChangeReason = "testReason",
                            ProviderApprovedDate = providerApprovedDate,
                            EmployerApprovedDate = employerApprovedDate
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
            result.OriginalTrainingPrice.Should().Be(apprenticeships[0].TrainingPrice);
            result.OriginalAssessmentPrice.Should().Be(apprenticeships[0].EndPointAssessmentPrice);
            result.OriginalTotalPrice.Should().Be(apprenticeships[0].TotalPrice);
            result.PendingTrainingPrice.Should().Be(10000);
            result.PendingAssessmentPrice.Should().Be(3000);
            result.PendingTotalPrice.Should().Be(13000);
            result.EffectiveFrom.Should().Be(effectiveFromDate);
            result.Reason.Should().Be("testReason");
            result.Ukprn.Should().Be(apprenticeships[0].Ukprn);
            result.ProviderApprovedDate.Should().Be(providerApprovedDate);
            result.EmployerApprovedDate.Should().Be(employerApprovedDate);
        }
    }
}
