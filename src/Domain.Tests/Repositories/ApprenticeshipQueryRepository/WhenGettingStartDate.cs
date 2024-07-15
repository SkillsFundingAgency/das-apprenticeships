using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.DataAccess;
using SFA.DAS.Apprenticeships.Domain.UnitTests.Helpers;
using SFA.DAS.Apprenticeships.TestHelpers;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Repositories.ApprenticeshipQueryRepository
{
    public class WhenGettingStartDate
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
            // Arrange
            var apprenticeshipKey = _fixture.Create<Guid>();
            SetUpApprenticeshipQueryRepository();

            //Act
            var result = await _sut.GetStartDate(_fixture.Create<Guid>());

            //Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task ThenTheCorrectStartDateIsReturned()
        {
            // Arrange
            var apprenticeshipKey = _fixture.Create<Guid>();
            SetUpApprenticeshipQueryRepository();

            await _dbContext.AddApprenticeship(apprenticeshipKey, false);
            var apprenticeship = _dbContext.Apprenticeships.Single(x => x.Key == apprenticeshipKey);
            var episode = _dbContext.Episodes.Single(x => x.ApprenticeshipKey == apprenticeshipKey);
            var episodePrice = _dbContext.EpisodePrices.Single(x => x.EpisodeKey == episode.Key);
            await _dbContext.AddApprenticeship(_fixture.Create<Guid>(), false);
            await _dbContext.AddApprenticeship(_fixture.Create<Guid>(), false);

            // Act
            var result = await _sut.GetStartDate(apprenticeshipKey);

            // Assert
            result.Should().NotBeNull();
            result.ActualStartDate.Should().Be(episodePrice.StartDate);
            result.PlannedEndDate.Should().Be(episodePrice.EndDate);
            result.AccountLegalEntityId.Should().Be(episode.AccountLegalEntityId);
            result.ApprenticeDateOfBirth.Should().Be(apprenticeship.DateOfBirth);
            result.CourseCode.Should().Be(episode.TrainingCode);
            result.CourseVersion.Should().Be(episode.TrainingCourseVersion);
        }

        private void SetUpApprenticeshipQueryRepository()
        {
            _dbContext = InMemoryDbContextCreator.SetUpInMemoryDbContext();
            var logger = Mock.Of<ILogger<Domain.Repositories.ApprenticeshipQueryRepository>>();
            _sut = new Domain.Repositories.ApprenticeshipQueryRepository(new Lazy<ApprenticeshipsDataContext>(_dbContext), logger);
        }
    }
}
