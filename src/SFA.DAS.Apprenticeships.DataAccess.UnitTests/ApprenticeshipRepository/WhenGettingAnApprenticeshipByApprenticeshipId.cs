using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Domain;
using SFA.DAS.Apprenticeships.Domain.Factories;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Apprenticeships.DataAccess.UnitTests.ApprenticeshipRepository
{
    public class WhenGettingAnApprenticeshipByApprenticeshipId
    {
        private Repositories.ApprenticeshipRepository _sut;
        private Fixture _fixture;
        private ApprenticeshipsDataContext _dbContext;
        private Mock<IDomainEventDispatcher> _domainEventDispatcher;

        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();
            _fixture.Customize(new ApprenticeshipCustomization());

            var options = new DbContextOptionsBuilder<ApprenticeshipsDataContext>().UseInMemoryDatabase("EmployerIncentivesDbContext" + Guid.NewGuid()).Options;
            _dbContext = new ApprenticeshipsDataContext(options);

            _domainEventDispatcher = new Mock<IDomainEventDispatcher>();

            _sut = new Repositories.ApprenticeshipRepository(new Lazy<ApprenticeshipsDataContext>(_dbContext), _domainEventDispatcher.Object, new ApprenticeshipFactory());
        }

        [TearDown]
        public void CleanUp()
        {
            _dbContext.Dispose();
        }

        [Test]
        public async Task Then_the_apprenticeship_is_retrieved_with_price_history()
        {
            // Arrange
            var expected = _fixture.Create<DataAccess.Entities.Apprenticeship.Apprenticeship>();
            var apprenticeshipId = expected.Approvals.First().ApprovalsApprenticeshipId;

            await _dbContext.AddAsync(expected);
            await _dbContext.SaveChangesAsync();

            // Act
            var actual = await _sut.GetByApprenticeshipId(apprenticeshipId);

            // Assert
            actual.Should().BeEquivalentTo(expected, opt => opt
                .Excluding(_ => _.Approvals)
                .Excluding(_ => _.PriceHistory)
            );
            actual.PriceHistory.Should().BeEquivalentTo(expected.PriceHistory, opt => opt
                .Excluding(_ => _.ApprenticeshipKey)
            );
        }
    }
}
