using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.DataAccess;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Repositories.ApprenticeshipQueryRepository
{
    public class WhenGettingPrice
    {
        private Domain.Repositories.ApprenticeshipQueryRepository _sut;
        private Fixture _fixture;
        private ApprenticeshipsDataContext _dbContext;

        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();

            var options = new DbContextOptionsBuilder<ApprenticeshipsDataContext>().UseInMemoryDatabase("ApprenticeshipsDbContext" + Guid.NewGuid()).Options;
            _dbContext = new ApprenticeshipsDataContext(options);

            _sut = new Domain.Repositories.ApprenticeshipQueryRepository(new Lazy<ApprenticeshipsDataContext>(_dbContext));
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
            var result = await _sut.GetPrice(_fixture.Create<Guid>());

            //Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task ThenTheCorrectPriceIsReturned()
        {
            // Arrange
            var apprenticeshipKey = _fixture.Create<Guid>();

            var apprenticeships = new[]
            {
                _fixture.Build<DataAccess.Entities.Apprenticeship.Apprenticeship>().With(x => x.Key, apprenticeshipKey).Create(),
                _fixture.Build<DataAccess.Entities.Apprenticeship.Apprenticeship>().With(x => x.Key, _fixture.Create<Guid>()).Create(),
                _fixture.Build<DataAccess.Entities.Apprenticeship.Apprenticeship>().With(x => x.Key, _fixture.Create<Guid>()).Create(),
                _fixture.Build<DataAccess.Entities.Apprenticeship.Apprenticeship>().With(x => x.Key, _fixture.Create<Guid>()).Create()
            };

            await _dbContext.AddRangeAsync(apprenticeships);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _sut.GetPrice(apprenticeshipKey);

            // Assert
            result.Should().NotBeNull();
            result.TotalPrice = apprenticeships[0].TotalPrice;
            result.AssessmentPrice = apprenticeships[0].EndPointAssessmentPrice;
            result.TrainingPrice = apprenticeships[0].TrainingPrice;
            result.FundingBandMaximum = apprenticeships[0].FundingBandMaximum;
            result.ApprenticeshipActualStartDate = apprenticeships[0].ActualStartDate;
            result.ApprenticeshipPlannedEndDate = apprenticeships[0].PlannedEndDate;
            result.AccountLegalEntityId = apprenticeships[0].AccountLegalEntityId;
        }
    }
}
