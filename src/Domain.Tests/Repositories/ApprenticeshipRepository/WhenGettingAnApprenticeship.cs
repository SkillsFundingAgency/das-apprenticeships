using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.DataAccess;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Factories;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.Infrastructure;
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
        private Mock<IAccountIdClaimsHandler> _accountIdClaimsHandler;
        private Mock<IAccountIdValidator> _accountIdValidator;

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
        public async Task Then_the_apprenticeship_is_retrieved()
        {
            // Arrange
            var expectedApprenticeship = ApprenticeshipDomainModel.Get(_fixture.Create<DataAccess.Entities.Apprenticeship.Apprenticeship>());
            SetUpApprenticeshipRepository(expectedApprenticeship);
            _apprenticeshipFactory.Setup(x => x.GetExisting(It.Is<DataAccess.Entities.Apprenticeship.Apprenticeship>(y =>
                    y.Key == expectedApprenticeship.Key &&
                    y.TrainingCode == expectedApprenticeship.TrainingCode &&
                    y.Uln == expectedApprenticeship.Uln
                ))).Returns(expectedApprenticeship);

            // Act
            await _sut.Add(expectedApprenticeship);
            var actualApprenticeship = await _sut.Get(expectedApprenticeship.Key);

            // Assert
            actualApprenticeship.Should().BeEquivalentTo(expectedApprenticeship);
        }

        private void SetUpApprenticeshipRepository(ApprenticeshipDomainModel testApprenticeship)
        {
            _domainEventDispatcher = new Mock<IDomainEventDispatcher>();
            _apprenticeshipFactory = new Mock<IApprenticeshipFactory>();
            _accountIdValidator = new Mock<IAccountIdValidator>();
            _accountIdClaimsHandler = AuthorizationHelper.MockAccountIdClaimsHandler(testApprenticeship.Ukprn, AccountIdClaimsType.Provider);
            _dbContext =
                InMemoryDbContextCreator.SetUpInMemoryDbContext(_accountIdClaimsHandler.Object);
            _sut = new Domain.Repositories.ApprenticeshipRepository(new Lazy<ApprenticeshipsDataContext>(_dbContext),
                _domainEventDispatcher.Object, _apprenticeshipFactory.Object, _accountIdValidator.Object);
        }
    }
}
