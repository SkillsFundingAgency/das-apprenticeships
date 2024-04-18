using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.DataAccess;

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
            var result = await _sut.GetStartDate(_fixture.Create<Guid>());

            //Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task ThenTheCorrectStartDateIsReturned()
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
            var result = await _sut.GetStartDate(apprenticeshipKey);

            // Assert
            result.Should().NotBeNull();
            result.ActualStartDate.Should().Be(apprenticeships[0].ActualStartDate);
            result.PlannedEndDate.Should().Be(apprenticeships[0].PlannedEndDate);
            result.AccountLegalEntityId.Should().Be(apprenticeships[0].AccountLegalEntityId);
            result.ApprenticeDateOfBirth.Should().Be(apprenticeships[0].DateOfBirth);
            result.CourseCode.Should().Be(apprenticeships[0].TrainingCode);
            result.CourseVersion.Should().Be(apprenticeships[0].TrainingCourseVersion);
        }
    }
}
