using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Factories;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;

namespace SFA.DAS.Apprenticeships.DataAccess.UnitTests.ApprenticeshipRepository
{
    public class WhenGettingAnApprenticeship
    {
        private Domain.Repositories.ApprenticeshipRepository _sut;
        private Fixture _fixture;
        private ApprenticeshipsDataContext _dbContext;
        private Mock<IDomainEventDispatcher> _domainEventDispatcher;
        private Mock<IApprenticeshipFactory> _apprenticeshipFactory;

        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();
            _fixture.Customize(new ApprenticeshipCustomization());

            var options = new DbContextOptionsBuilder<ApprenticeshipsDataContext>().UseInMemoryDatabase("EmployerIncentivesDbContext" + Guid.NewGuid()).Options;
            _dbContext = new ApprenticeshipsDataContext(options);

            _domainEventDispatcher = new Mock<IDomainEventDispatcher>();
            _apprenticeshipFactory = new Mock<IApprenticeshipFactory>();

            _sut = new Domain.Repositories.ApprenticeshipRepository(new Lazy<ApprenticeshipsDataContext>(_dbContext), _domainEventDispatcher.Object, _apprenticeshipFactory.Object);
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
            var expectedApprenticeship = ApprenticeshipDomainModel.Get(_fixture.Create<Apprenticeship>());

            _apprenticeshipFactory.Setup(x => x.GetExisting(It.Is<Apprenticeship>(y =>
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
    }
}
