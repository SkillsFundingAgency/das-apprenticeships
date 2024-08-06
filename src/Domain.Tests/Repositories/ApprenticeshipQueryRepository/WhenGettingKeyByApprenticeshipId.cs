using System;
using System.Collections.Generic;
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
    public class WhenGettingKeyByApprenticeshipId
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
        public async Task ThenReturnNullWhenNoRecordFoundWithApprenticeshipId()
        {
            //Arrange
            SetUpApprenticeshipQueryRepository();

            //Act
            var result = await _sut.GetKeyByApprenticeshipId(_fixture.Create<long>());

            //Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task ThenTheCorrectApprenticeshipKeyIsReturned()
        {
            //Arrange
            SetUpApprenticeshipQueryRepository();

            //Act
            var approvalsApprenticeshipId = _fixture.Create<long>();
            var expectedApprenticeshipKey = _fixture.Create<Guid>();
            await _dbContext.AddApprenticeship(expectedApprenticeshipKey, false, approvalsApprenticeshipId: approvalsApprenticeshipId);
            await _dbContext.AddApprenticeship(_fixture.Create<Guid>(), false, approvalsApprenticeshipId: _fixture.Create<long>());

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

        private void SetUpApprenticeshipQueryRepository()
        {
            _dbContext = InMemoryDbContextCreator.SetUpInMemoryDbContext();
            var logger = Mock.Of<ILogger<Domain.Repositories.ApprenticeshipQueryRepository>>();
            _sut = new Domain.Repositories.ApprenticeshipQueryRepository(new Lazy<ApprenticeshipsDataContext>(_dbContext), logger);
        }
    }
}