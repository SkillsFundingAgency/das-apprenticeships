using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.DataAccess;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Factories;
using SFA.DAS.Apprenticeships.TestHelpers;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Repositories.ApprenticeshipRepository
{
    public class WhenGettingAnApprenticeship
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
        public async Task ThenApprenticeshipIsRetrieved()
        {
            // Arrange
            SetUpApprenticeshipRepository();
            var expectedApprenticeship = ApprenticeshipDomainModel.Get(_fixture.Create<DataAccess.Entities.Apprenticeship.Apprenticeship>());
            _apprenticeshipFactory
                .Setup(x => x.GetExisting(It.IsAny<DataAccess.Entities.Apprenticeship.Apprenticeship>())).Returns(expectedApprenticeship);

            // Act
            await _sut.Add(expectedApprenticeship);
            var actualApprenticeship = await _sut.Get(expectedApprenticeship.Key);

            // Assert
            actualApprenticeship.Should().BeEquivalentTo(expectedApprenticeship);
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
