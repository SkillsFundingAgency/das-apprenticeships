using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.TestHelpers;
using SFA.DAS.Apprenticeships.Infrastructure;
using AccountIdClaims = SFA.DAS.Apprenticeships.TestHelpers.AccountIdClaims;

namespace SFA.DAS.Apprenticeships.DataAccess.UnitTests.ApprenticeshipQueryRepository
{
    public class WhenGettingAllApprenticeships
    {
        private Domain.Repositories.ApprenticeshipQueryRepository _sut;
        private Fixture _fixture;
        private ApprenticeshipsDataContext _dbContext;
        private Mock<IAccountIdClaimsHandler> _accountIdClaimsHandler;

        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();
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
            var providerInTest = ukprns[1];
            SetUpApprenticeshipQueryRepository(providerInTest);

            var apprenticeships = new[]
            {
                CreateApprenticeshipWithUkprn(ulns[0], ukprns[0]),
                CreateApprenticeshipWithUkprn(ulns[1], providerInTest),
                CreateApprenticeshipWithUkprn(ulns[2], providerInTest),
            };
            await _dbContext.AddRangeAsync(apprenticeships);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _sut.GetAll(providerInTest, null);

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
            var providerInTest = ukprns[1];
            SetUpApprenticeshipQueryRepository(providerInTest);

            var apprenticeships = new[]
            {
                CreateApprenticeshipWithUkPrnAndFundingPlatform(ulns[0], ukprns[0], FundingPlatform.DAS),
                CreateApprenticeshipWithUkPrnAndFundingPlatform(ulns[1], providerInTest, FundingPlatform.SLD),
                CreateApprenticeshipWithUkPrnAndFundingPlatform(ulns[2], providerInTest, FundingPlatform.DAS),
                CreateApprenticeshipWithUkPrnAndFundingPlatform(ulns[3], providerInTest, FundingPlatform.SLD),
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
        
        private void SetUpApprenticeshipQueryRepository(long ukprn)
        {
            _accountIdClaimsHandler =
                AccountIdClaims.MockAccountIdClaimsHandler(ukprn, AccountIdClaimsType.Provider);
            _dbContext =
                InMemoryDbContextCreator.SetUpInMemoryDbContext(_accountIdClaimsHandler.Object);
            _sut = new Domain.Repositories.ApprenticeshipQueryRepository(new Lazy<ApprenticeshipsDataContext>(_dbContext));
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
                .With(x => x.UKPRN, ukprn)
                .With(x => x.Approvals, new List<Approval>() { new()
                {
                    FundingPlatform = fundingPlatform,
                    LegalEntityName = "legalEntityName"
                } })
                .Create();
        }
    }
}
