using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Castle.Core.Logging;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.DataAccess.UnitTests.ApprenticeshipQueryRepository
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

            var options = new DbContextOptionsBuilder<ApprenticeshipsDataContext>().UseInMemoryDatabase("ApprenticeshipsDbContext" + Guid.NewGuid()).Options;
            _dbContext = new ApprenticeshipsDataContext(options);
            var logger = Mock.Of<ILogger<Domain.Repositories.ApprenticeshipQueryRepository>>();

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
            var ukprns = _fixture.CreateMany<long>(2).ToList();
            var ulns = _fixture.CreateMany<string>(3).ToList();
            var apprenticeships = new[]
            {
                CreateApprenticeshipWithUkprn(ulns[0], ukprns[0]),
                CreateApprenticeshipWithUkprn(ulns[1], ukprns[1]),
                CreateApprenticeshipWithUkprn(ulns[2], ukprns[1]),
            };
            await _dbContext.AddRangeAsync(apprenticeships);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _sut.GetAll(ukprns[1], null);

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(2);
            result.Should().NotContain(x => x.Uln == ulns[0]);
            result.Should().Contain(x => x.Uln == ulns[1]);
            result.Should().Contain(x => x.Uln == ulns[2]);
        }

        [Test]
        public async Task And_filtering_by_funding_platform_Then_the_correct_apprenticeships_are_retrieved()
        {
            // Arrange
            var ukprns = _fixture.CreateMany<long>(2).ToList();
            var ulns = _fixture.CreateMany<string>(4).ToList();
            var apprenticeships = new[]
            {
                CreateApprenticeshipWithUkPrnAndFundingPlatform(ulns[0], ukprns[0], FundingPlatform.DAS),
                CreateApprenticeshipWithUkPrnAndFundingPlatform(ulns[1], ukprns[1], FundingPlatform.SLD),
                CreateApprenticeshipWithUkPrnAndFundingPlatform(ulns[2], ukprns[1], FundingPlatform.DAS),
                CreateApprenticeshipWithUkPrnAndFundingPlatform(ulns[3], ukprns[1], FundingPlatform.SLD),
            };
            await _dbContext.AddRangeAsync(apprenticeships);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _sut.GetAll(ukprns[1], FundingPlatform.SLD);

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(2);
            result.Should().NotContain(x => x.Uln == ulns[0]);
            result.Should().Contain(x => x.Uln == ulns[1]);
            result.Should().NotContain(x => x.Uln == ulns[2]);
            result.Should().Contain(x => x.Uln == ulns[3]);

        }

        private Apprenticeship CreateApprenticeshipWithUkprn(string uln, long ukprn)
        {
            return CreateApprenticeshipWithUkPrnAndFundingPlatform(uln, ukprn, _fixture.Create<FundingPlatform>());
        }

        private Apprenticeship CreateApprenticeshipWithUkPrnAndFundingPlatform(string uln, long ukprn, FundingPlatform fundingPlatform)
        {
            return _fixture
                .Build<Apprenticeship>()
                .With(x => x.Uln, uln)
                .With(x => x.Ukprn, ukprn)
                .With(x => x.Approvals, new List<Approval>() { new()
                {
                    FundingPlatform = fundingPlatform,
                    LegalEntityName = "legalEntityName"
                } })
                .Create();
        }
    }
}
