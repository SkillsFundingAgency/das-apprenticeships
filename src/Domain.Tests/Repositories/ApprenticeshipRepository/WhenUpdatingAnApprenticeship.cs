using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.DataAccess;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Factories;
using SFA.DAS.Apprenticeships.TestHelpers;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Repositories.ApprenticeshipRepository
{
    public class WhenUpdatingAnApprenticeship
    {
        private Domain.Repositories.ApprenticeshipRepository _sut;
        private Fixture _fixture;
        private ApprenticeshipsDataContext _dbContext;
        private Mock<IDomainEventDispatcher> _domainEventDispatcher;
        private Mock<IApprenticeshipFactory> _apprenticeshipFactory;
        private Mock<IAccountIdAuthorizer> _accountIdAuthorizer;

        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();
            _fixture.Customize(new ApprenticeshipCustomization());
        }

        [TearDown]
        public void CleanUp()
        {
            _dbContext.Dispose();
        }

        [Test]
        public async Task ThenAccountIdValidationIsPerformed()
        {
            //todo fix once authorization logic is fixed
            //// Arrange
            //var apprenticeshipEntity = _fixture.Create<DataAccess.Entities.Apprenticeship.Apprenticeship>();
            //await SetUpApprenticeshipRepository(apprenticeshipEntity);
            //var apprenticeshipDomainModel = ApprenticeshipDomainModel.Get(apprenticeshipEntity);

            //// Act
            //await _sut.Update(apprenticeshipDomainModel);
            
            //// Assert
            //_accountIdAuthorizer.Verify(x => x.AuthorizeAccountId(apprenticeshipEntity), Times.Once());
        }

        [Test]
        public async Task ThenApprenticeshipUpdatedInDataStore()
        {
            // Arrange
            var apprenticeshipEntity = _fixture.Create<DataAccess.Entities.Apprenticeship.Apprenticeship>();
            await SetUpApprenticeshipRepository(apprenticeshipEntity);
            apprenticeshipEntity.DateOfBirth = _fixture.Create<DateTime>();
            var domainModel = ApprenticeshipDomainModel.Get(apprenticeshipEntity);

            // Act
            await _sut.Update(domainModel);
            
            // Assert
            _dbContext.ApprenticeshipsDbSet.Count().Should().Be(1);
            var storedApprenticeship = _dbContext.ApprenticeshipsDbSet.Single();
            apprenticeshipEntity.Should().BeEquivalentTo(storedApprenticeship);
        }

        [Test]
        public async Task ThenEpisodeUpdatedInDataStore()
        {
            // Arrange
            var apprenticeshipEntity = _fixture.Create<DataAccess.Entities.Apprenticeship.Apprenticeship>();
            var ukprn = _fixture.Create<long>();
            apprenticeshipEntity.Episodes = new List<Episode> { new() { Ukprn = ukprn, LegalEntityName = "fake_name"}};
            await SetUpApprenticeshipRepository(apprenticeshipEntity);
            apprenticeshipEntity.Episodes.Single(x => x.Ukprn == ukprn).LegalEntityName= "alternative_name";
            var domainModel = ApprenticeshipDomainModel.Get(apprenticeshipEntity);

            // Act
            await _sut.Update(domainModel);
            
            // Assert
            _dbContext.ApprenticeshipsDbSet.Count().Should().Be(1);
            _dbContext.Episodes.Count().Should().Be(1);
            apprenticeshipEntity.Episodes.Single().LegalEntityName.Should().Be("alternative_name");
        }

        private async Task SetUpApprenticeshipRepository(DataAccess.Entities.Apprenticeship.Apprenticeship existingApprenticeship = null)
        {
            _domainEventDispatcher = new Mock<IDomainEventDispatcher>();
            _apprenticeshipFactory = new Mock<IApprenticeshipFactory>();
            _accountIdAuthorizer = new Mock<IAccountIdAuthorizer>();
            _dbContext =
                InMemoryDbContextCreator.SetUpInMemoryDbContext();
            if (existingApprenticeship != null)
            {
                await _dbContext.ApprenticeshipsDbSet.AddAsync(existingApprenticeship);
                await _dbContext.SaveChangesAsync();
            }
            _sut = new Domain.Repositories.ApprenticeshipRepository(new Lazy<ApprenticeshipsDataContext>(_dbContext),
                _domainEventDispatcher.Object, _apprenticeshipFactory.Object, _accountIdAuthorizer.Object);
        }
    }
}
