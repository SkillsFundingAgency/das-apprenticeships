using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.DataAccess;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Events;
using SFA.DAS.Apprenticeships.Domain.Factories;
using SFA.DAS.Apprenticeships.TestHelpers;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Repositories.ApprenticeshipRepository
{
    public class WhenAddingAnApprenticeship
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
            // Arrange
            var apprenticeship = ApprenticeshipDomainModel.Get(_fixture.Create<DataAccess.Entities.Apprenticeship.Apprenticeship>());
            SetUpApprenticeshipRepository();

            // Act
            await _sut.Add(apprenticeship);
            
            // Assert
            var entity = apprenticeship.GetEntity();
            _accountIdAuthorizer.Verify(x => x.AuthorizeAccountId(entity), Times.Once());
        }

        [Test]
        public async Task ThenApprenticeshipAddedToDataStore()
        {
            // Arrange
            var testApprenticeship = ApprenticeshipDomainModel.Get(_fixture.Create<DataAccess.Entities.Apprenticeship.Apprenticeship>());
            SetUpApprenticeshipRepository();

            // Act
            await _sut.Add(testApprenticeship);
            
            // Assert
            _dbContext.ApprenticeshipsDbSet.Count().Should().Be(1);

            var storedApprenticeship = _dbContext.ApprenticeshipsDbSet.Single();
            var expectedModel = testApprenticeship.GetEntity();

            expectedModel.Should().BeEquivalentTo(storedApprenticeship);
        }

        [Test]
        public async Task ThenEpisodeAddedToDataStore()
        {
            // Arrange
            var apprenticeshipEntity = _fixture.Create<DataAccess.Entities.Apprenticeship.Apprenticeship>();
            var testApprenticeship = ApprenticeshipDomainModel.Get(apprenticeshipEntity);
            SetUpApprenticeshipRepository();
            var expectedEpisode = EpisodeDomainModel.Get(_fixture.Create<Episode>());
            var expectedEpisodePrice = EpisodePriceDomainModel.Get(_fixture.Create<EpisodePrice>());
            testApprenticeship.AddEpisode(
                expectedEpisode.Ukprn, 
                expectedEpisode.EmployerAccountId, 
                expectedEpisodePrice.StartDate, 
                expectedEpisodePrice.EndDate, 
                expectedEpisodePrice.TotalPrice, 
                expectedEpisodePrice.TrainingPrice, 
                expectedEpisodePrice.EndPointAssessmentPrice, 
                expectedEpisode.FundingType,
                expectedEpisode.FundingPlatform,
                expectedEpisodePrice.FundingBandMaximum, 
                expectedEpisode.FundingEmployerAccountId, 
                expectedEpisode.LegalEntityName, 
                expectedEpisode.AccountLegalEntityId, 
                expectedEpisode.TrainingCode, 
                expectedEpisode.TrainingCourseVersion);

            // Act
            await _sut.Add(testApprenticeship);
            
            // Assert
            _dbContext.Episodes.Count().Should().Be(1);
            var storedEpisode = _dbContext.Episodes.Single();
            storedEpisode.Should().BeEquivalentTo(expectedEpisode);
        }

        [Test]
        public async Task ThenDomainEventsPublished()
        {
            // Arrange
            var apprenticeshipEntity = _fixture.Create<DataAccess.Entities.Apprenticeship.Apprenticeship>();
            var testApprenticeship = ApprenticeshipDomainModel.Get(apprenticeshipEntity);
            SetUpApprenticeshipRepository();
			
            // Act
            await _sut.Add(testApprenticeship);
            
            // Assert
            _domainEventDispatcher.Verify(x => x.Send(It.Is<ApprenticeshipCreated>(e => e.ApprenticeshipKey == testApprenticeship.Key), It.IsAny<CancellationToken>()), Times.Once());
        }

        private void SetUpApprenticeshipRepository()
        {
            _domainEventDispatcher = new Mock<IDomainEventDispatcher>();
            _apprenticeshipFactory = new Mock<IApprenticeshipFactory>();
            _accountIdAuthorizer = new Mock<IAccountIdAuthorizer>();
            _dbContext =
                InMemoryDbContextCreator.SetUpInMemoryDbContext();
            _sut = new Domain.Repositories.ApprenticeshipRepository(new Lazy<ApprenticeshipsDataContext>(_dbContext),
                _domainEventDispatcher.Object, _apprenticeshipFactory.Object, _accountIdAuthorizer.Object);
        }
    }
}
