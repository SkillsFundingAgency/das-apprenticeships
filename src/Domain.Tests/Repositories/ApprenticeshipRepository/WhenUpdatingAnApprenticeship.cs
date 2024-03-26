using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
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
            // Arrange
            var apprenticeshipEntity = _fixture.Create<DataAccess.Entities.Apprenticeship.Apprenticeship>();
            await SetUpApprenticeshipRepository(apprenticeshipEntity);
            var apprenticeshipDomainModel = ApprenticeshipDomainModel.Get(apprenticeshipEntity);

            // Act
            await _sut.Update(apprenticeshipDomainModel);
            
            // Assert
            _accountIdAuthorizer.Verify(x => x.ValidateAccountIds(apprenticeshipEntity), Times.Once());
        }

        [Test]
        public async Task ThenApprenticeshipUpdatedInDataStore()
        {
            // Arrange
            var apprenticeshipEntity = _fixture.Create<DataAccess.Entities.Apprenticeship.Apprenticeship>();
            await SetUpApprenticeshipRepository(apprenticeshipEntity);
            apprenticeshipEntity.Ukprn = 123153290480;
            var domainModel = ApprenticeshipDomainModel.Get(apprenticeshipEntity);

            // Act
            await _sut.Update(domainModel);
            
            // Assert
            _dbContext.Apprenticeships.Count().Should().Be(1);
            var storedApprenticeship = _dbContext.Apprenticeships.Include(x => x.Approvals).Include(x => x.PriceHistories).Single();
            apprenticeshipEntity.Should().BeEquivalentTo(storedApprenticeship);
        }

        [Test]
        public async Task ThenApprovalUpdatedInDataStore()
        {
            // Arrange
            var apprenticeshipEntity = _fixture.Create<DataAccess.Entities.Apprenticeship.Apprenticeship>();
            var approvalsApprenticeshipId = _fixture.Create<long>();
            apprenticeshipEntity.Approvals = new List<Approval>(){ new() { ApprovalsApprenticeshipId = approvalsApprenticeshipId, LegalEntityName = "fake_name"}};
            await SetUpApprenticeshipRepository(apprenticeshipEntity);
            apprenticeshipEntity.Approvals.Single(x => x.ApprovalsApprenticeshipId == approvalsApprenticeshipId)
                .ApprovalsApprenticeshipId = 12345;
            var domainModel = ApprenticeshipDomainModel.Get(apprenticeshipEntity);

            // Act
            await _sut.Update(domainModel);
            
            // Assert
            _dbContext.Apprenticeships.Count().Should().Be(1);
            _dbContext.Approvals.Count().Should().Be(1);
            apprenticeshipEntity.Approvals.Single().ApprovalsApprenticeshipId.Should().Be(12345);
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
                await _dbContext.Apprenticeships.AddAsync(existingApprenticeship);
                await _dbContext.SaveChangesAsync();
            }
            _sut = new Domain.Repositories.ApprenticeshipRepository(new Lazy<ApprenticeshipsDataContext>(_dbContext),
                _domainEventDispatcher.Object, _apprenticeshipFactory.Object, _accountIdAuthorizer.Object);
        }
    }
}
