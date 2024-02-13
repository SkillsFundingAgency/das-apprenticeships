using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.DataAccess;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Repositories.ApprenticeshipQueryRepository
{
    public class WhenGettingAllApprenticeships
    {
        private Domain.Repositories.ApprenticeshipQueryRepository _sut;
        private Fixture _fixture;
        private ApprenticeshipsDataContext _dbContext;

        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();
            var logger = Mock.Of<ILogger<Domain.Repositories.ApprenticeshipQueryRepository>>();

            var options = new DbContextOptionsBuilder<ApprenticeshipsDataContext>().UseInMemoryDatabase("ApprenticeshipsDbContext" + Guid.NewGuid()).Options;
            _dbContext = new ApprenticeshipsDataContext(options);

            _sut = new Domain.Repositories.ApprenticeshipQueryRepository(new Lazy<ApprenticeshipsDataContext>(_dbContext), logger);
        }

        [TearDown]
        public void CleanUp()
        {
            _dbContext.Dispose();
        }

        [Test]
        public async Task Then_the_correct_apprenticeships_for_the_ukprn_are_retrieved()
        {
            // Arrange
            var ukprn1 = _fixture.Create<long>();
            var ukprn2 = _fixture.Create<long>();

            var apprenticeships = new DataAccess.Entities.Apprenticeship.Apprenticeship[]
            {
                _fixture.Build<DataAccess.Entities.Apprenticeship.Apprenticeship>().With(x => x.Uln, "1001").With(x => x.FirstName, "Ron").With(x => x.LastName, "Swanson").With(x => x.Approvals, new List<Approval>() { new Approval() { UKPRN = ukprn1, LegalEntityName = "" } }).Create(),
                _fixture.Build<DataAccess.Entities.Apprenticeship.Apprenticeship>().With(x => x.Uln, "1002").With(x => x.FirstName, "Stepan").With(x => x.LastName, "Tominski").With(x => x.Approvals, new List<Approval>(){ new Approval() { UKPRN =  ukprn2, LegalEntityName = "" } }).Create(),
                _fixture.Build<DataAccess.Entities.Apprenticeship.Apprenticeship>().With(x => x.Uln, "1003").With(x => x.FirstName, "Lucy").With(x => x.LastName, "Rogers").With(x => x.Approvals, new List<Approval>(){ new Approval() { UKPRN =  ukprn1, LegalEntityName = "" }, new Approval() { UKPRN =  ukprn2, LegalEntityName = ""} }).Create(),
                _fixture.Build<DataAccess.Entities.Apprenticeship.Apprenticeship>().With(x => x.Uln, "1004").With(x => x.FirstName, "Shamil").With(x => x.LastName, "Ahmur").With(x => x.Approvals, new List<Approval>(){ new Approval() { UKPRN =  ukprn1, LegalEntityName = "" }, new Approval() { UKPRN = ukprn1, LegalEntityName = "" } }).Create(),
                _fixture.Build<DataAccess.Entities.Apprenticeship.Apprenticeship>().With(x => x.Uln, "1005").With(x => x.FirstName, "Tracey").With(x => x.LastName, "Smith").With(x => x.Approvals, new List<Approval>(){ new Approval() { UKPRN =  ukprn2, LegalEntityName = "" }, new Approval() { UKPRN = ukprn2, LegalEntityName = "" } }).Create(),
            };

            await _dbContext.AddRangeAsync(apprenticeships);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _sut.GetAll(ukprn1, null);

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(3);
            result.Any(x => x.Uln == apprenticeships[0].Uln && x.FirstName == apprenticeships[0].FirstName && x.LastName == apprenticeships[0].LastName).Should().BeTrue();
            result.Any(x => x.Uln == apprenticeships[2].Uln && x.FirstName == apprenticeships[2].FirstName && x.LastName == apprenticeships[2].LastName).Should().BeTrue();
            result.Any(x => x.Uln == apprenticeships[3].Uln && x.FirstName == apprenticeships[3].FirstName && x.LastName == apprenticeships[3].LastName).Should().BeTrue();
            result.Any(x => x.Uln == apprenticeships[1].Uln && x.FirstName == apprenticeships[1].FirstName && x.LastName == apprenticeships[1].LastName).Should().BeFalse();
            result.Any(x => x.Uln == apprenticeships[4].Uln && x.FirstName == apprenticeships[4].FirstName && x.LastName == apprenticeships[4].LastName).Should().BeFalse();
        }


        [Test]
        public async Task And_filtering_by_funding_platform_Then_the_correct_apprenticeships_are_retrieved()
        {
            // Arrange
            var ukprn = _fixture.Create<long>();

            var apprenticeships = new DataAccess.Entities.Apprenticeship.Apprenticeship[]
            {
                _fixture.Build<DataAccess.Entities.Apprenticeship.Apprenticeship>().With(x => x.Uln, "1001").With(x => x.FirstName, "Ron").With(x => x.LastName, "Swanson").With(x => x.Approvals, new List<Approval>() { new Approval() { UKPRN = ukprn, FundingPlatform = FundingPlatform.DAS, LegalEntityName = "" } }).Create(),
                _fixture.Build<DataAccess.Entities.Apprenticeship.Apprenticeship>().With(x => x.Uln, "1002").With(x => x.FirstName, "Stepan").With(x => x.LastName, "Tominski").With(x => x.Approvals, new List<Approval>(){ new Approval() { UKPRN =  ukprn, FundingPlatform = FundingPlatform.SLD, LegalEntityName = "" } }).Create(),
                _fixture.Build<DataAccess.Entities.Apprenticeship.Apprenticeship>().With(x => x.Uln, "1003").With(x => x.FirstName, "Lucy").With(x => x.LastName, "Rogers").With(x => x.Approvals, new List<Approval>(){ new Approval() { UKPRN =  ukprn, FundingPlatform = FundingPlatform.SLD, LegalEntityName = "" }, new Approval() { UKPRN =  ukprn, FundingPlatform = FundingPlatform.SLD, LegalEntityName = ""} }).Create(),
                _fixture.Build<DataAccess.Entities.Apprenticeship.Apprenticeship>().With(x => x.Uln, "1004").With(x => x.FirstName, "Shamil").With(x => x.LastName, "Ahmur").With(x => x.Approvals, new List<Approval>(){ new Approval() { UKPRN =  ukprn, FundingPlatform = FundingPlatform.SLD, LegalEntityName = "" }, new Approval() { UKPRN = ukprn, FundingPlatform = FundingPlatform.DAS, LegalEntityName = "" } }).Create(),                _fixture.Build<DataAccess.Entities.Apprenticeship.Apprenticeship>().With(x => x.Uln, "1005").With(x => x.FirstName, "Tracey").With(x => x.LastName, "Smith").With(x => x.Approvals, new List<Approval>(){ new Approval() { UKPRN =  ukprn, FundingPlatform = FundingPlatform.DAS, LegalEntityName = "" }, new Approval() { UKPRN = ukprn, FundingPlatform = FundingPlatform.DAS, LegalEntityName = "" } }).Create(),
            };

            await _dbContext.AddRangeAsync(apprenticeships);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _sut.GetAll(ukprn, FundingPlatform.DAS);

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(3);
            result.Any(x => x.Uln == apprenticeships[0].Uln && x.FirstName == apprenticeships[0].FirstName && x.LastName == apprenticeships[0].LastName).Should().BeTrue();
            result.Any(x => x.Uln == apprenticeships[3].Uln && x.FirstName == apprenticeships[3].FirstName && x.LastName == apprenticeships[3].LastName).Should().BeTrue();
            result.Any(x => x.Uln == apprenticeships[4].Uln && x.FirstName == apprenticeships[4].FirstName && x.LastName == apprenticeships[4].LastName).Should().BeTrue();
            result.Any(x => x.Uln == apprenticeships[1].Uln && x.FirstName == apprenticeships[1].FirstName && x.LastName == apprenticeships[1].LastName).Should().BeFalse();
            result.Any(x => x.Uln == apprenticeships[2].Uln && x.FirstName == apprenticeships[2].FirstName && x.LastName == apprenticeships[2].LastName).Should().BeFalse();
        }
    }
}
