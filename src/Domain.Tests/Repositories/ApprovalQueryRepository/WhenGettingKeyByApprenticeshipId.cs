using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.DataAccess;
using SFA.DAS.Apprenticeships.TestHelpers;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Repositories.ApprovalQueryRepository
{
    public class WhenGettingKeyByApprenticeshipId
    {
        private Domain.Repositories.ApprovalQueryRepository _sut;
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
        public async Task ThenReturnNullWhenNoRecordFoundWithApprenticeshipId()
        {
            //Arrange
            SetUpApprovalQueryRepository();
            
            //Act
            var result = await _sut.GetKeyByApprenticeshipId(_fixture.Create<long>());

            //Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task ThenTheCorrectApprenticeshipKeyIsReturned()
        {
            //Arrange
            SetUpApprovalQueryRepository();
            
            //Act
            var approvalsApprenticeshipId = _fixture.Create<long>();
            var expectedApprenticeshipKey = _fixture.Create<Guid>();
            var apprenticeships = new List<DataAccess.Entities.Apprenticeship.Apprenticeship>
            {
                CreateApprenticeshipWithApproval(expectedApprenticeshipKey, approvalsApprenticeshipId), 
                CreateApprenticeshipWithApproval(_fixture.Create<Guid>(), _fixture.Create<long>())
            };
            await _dbContext.AddRangeAsync(apprenticeships);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _sut.GetKeyByApprenticeshipId(approvalsApprenticeshipId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(expectedApprenticeshipKey);
        }

        private DataAccess.Entities.Apprenticeship.Apprenticeship CreateApprenticeshipWithApproval(Guid apprenticeshipKey, long apprenticeshipId)
        {
            return _fixture.Build<DataAccess.Entities.Apprenticeship.Apprenticeship>()
                .With(x => x.Key, apprenticeshipKey)
                .Create();
        }

        private void SetUpApprovalQueryRepository()
        {
            _dbContext = InMemoryDbContextCreator.SetUpInMemoryDbContext();
            _sut = new Domain.Repositories.ApprovalQueryRepository(new Lazy<ApprenticeshipsDataContext>(_dbContext));
        }
    }
}