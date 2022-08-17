using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Domain;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Models;
using SFA.DAS.Apprenticeships.Domain.Factories;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;

namespace SFA.DAS.Apprenticeships.DataAccess.UnitTests.ApprenticeshipRepository
{
    public class WhenAddingAnApprenticeship
    {
        private Repositories.ApprenticeshipRepository _sut;
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

            _sut = new Repositories.ApprenticeshipRepository(new Lazy<ApprenticeshipsDataContext>(_dbContext), _domainEventDispatcher.Object, _apprenticeshipFactory.Object);
        }

        [TearDown]
        public void CleanUp()
        {
            _dbContext.Dispose();
        }

        [Test]
        public async Task Then_the_apprenticeship_is_added_to_the_data_store()
        {
            // Arrange
            var testApprenticeship = _fixture.Create<Apprenticeship>();
            
            // Act
            await _sut.Add(testApprenticeship);
            await _dbContext.SaveChangesAsync();

            // Assert
            _dbContext.Apprenticeships.Count().Should().Be(1);

            var storedApprenticeship = _dbContext.Apprenticeships.Single();
            var expectedModel = testApprenticeship.GetModel();

            expectedModel.Should().BeEquivalentTo(storedApprenticeship);
        }

        [Test]
        public async Task Then_the_approval_is_added_to_the_data_store()
        {
            // Arrange
            var testApprenticeship = _fixture.Create<Apprenticeship>();
            var expectedApproval = _fixture.Create<ApprovalModel>();

            // Act
            testApprenticeship.AddApproval(expectedApproval.ApprovalsApprenticeshipId, expectedApproval.UKPRN, expectedApproval.EmployerAccountId, expectedApproval.LegalEntityName, expectedApproval.ActualStartDate, expectedApproval.PlannedEndDate, expectedApproval.AgreedPrice, expectedApproval.FundingEmployerAccountId, expectedApproval.FundingType, expectedApproval.FundingBandMaximum);
            await _sut.Add(testApprenticeship);
            await _dbContext.SaveChangesAsync();

            // Assert
            _dbContext.Approvals.Count().Should().Be(1);

            var storedApproval = _dbContext.Approvals.Single();
            
            storedApproval.Should().BeEquivalentTo(expectedApproval, opts => opts.Excluding(x => x.Id));
        }
    }
}
