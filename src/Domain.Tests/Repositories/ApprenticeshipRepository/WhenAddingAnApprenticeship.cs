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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
            var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
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
            var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
            SetUpApprenticeshipRepository();

            // Act
            await _sut.Add(apprenticeship);
            
            // Assert
            _dbContext.ApprenticeshipsDbSet.Count().Should().Be(1);

            var storedApprenticeship = _dbContext.ApprenticeshipsDbSet.Single();
            var expectedModel = apprenticeship.GetEntity();

            expectedModel.Should().BeEquivalentTo(storedApprenticeship);
        }

        [Test]
        public async Task ThenEpisodeAddedToDataStore()
        {
            // Arrange
            var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
            SetUpApprenticeshipRepository();
            var episodePrice = _fixture.Build<EpisodePrice>().With(x => x.IsDeleted, false).Create();
            var episode = EpisodeDomainModel.Get(_fixture.Build<Episode>()
                .With(x => x.Prices, new List<EpisodePrice>(){ episodePrice })
                .With(x => x.PaymentsFrozen, false)
                .With(x => x.LearningStatus, "Active")
                .With(x => x.LastDayOfLearning, (DateTime?)null)
                .Create());

            apprenticeship.AddEpisode(
                episode.Ukprn,
                episode.EmployerAccountId,
                episodePrice.StartDate,
                episodePrice.EndDate,
                episodePrice.TotalPrice,
                episodePrice.TrainingPrice,
                episodePrice.EndPointAssessmentPrice,
                episode.FundingType,
                episode.FundingPlatform,
                episodePrice.FundingBandMaximum,
                episode.FundingEmployerAccountId,
                episode.LegalEntityName,
                episode.AccountLegalEntityId,
                episode.TrainingCode,
                episode.TrainingCourseVersion);

            // Act
            await _sut.Add(apprenticeship);
            
            // Assert
            _dbContext.Episodes.Count().Should().Be(1);
            var storedEpisode = _dbContext.Episodes.Single();
            storedEpisode.Should().BeEquivalentTo(episode, x => x
                .Excluding(y => y.Key)
                .Excluding(y => y.LatestPrice)
                .Excluding(y => y.EpisodePrices)
                .Excluding(y => y.FirstPrice)
                .Excluding(y => y.ActiveEpisodePrices)
                .Excluding(y => y.LearningStatus));
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
