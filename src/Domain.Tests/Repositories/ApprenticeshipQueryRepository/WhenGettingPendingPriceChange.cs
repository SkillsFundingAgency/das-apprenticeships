using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.DataAccess;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.Infrastructure;
using SFA.DAS.Apprenticeships.TestHelpers;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Repositories.ApprenticeshipQueryRepository
{
    public class WhenGettingPendingPriceChange
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
        public async Task ThenReturnNullWhenNoApprenticeshipFoundWithApprenticeshipKey()
        {
            //Arrange
            SetUpApprenticeshipQueryRepository(_fixture.Create<long>());

            //Act
            var result = await _sut.GetPendingPriceChange(_fixture.Create<Guid>());

            //Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task ThenReturnNullWhenNoPriceHistoryRecordsFoundForExistingApprenticeship()
        {
            //Arrange
            SetUpApprenticeshipQueryRepository(_fixture.Create<long>());

            //Act
            var apprenticeshipKey = _fixture.Create<Guid>();
            var otherApprenticeshipKey1 = _fixture.Create<Guid>();
            var otherApprenticeshipKey2 = _fixture.Create<Guid>();
            
            var apprenticeships = new[]
            {
                _fixture.Build<DataAccess.Entities.Apprenticeship.Apprenticeship>()
                    .With(x => x.Key, otherApprenticeshipKey1)
                    .With(x => x.PriceHistories, new List<PriceHistory>() { new() { ApprenticeshipKey = otherApprenticeshipKey1}})
                    .Create(), 
                _fixture.Build<DataAccess.Entities.Apprenticeship.Apprenticeship>()
                    .With(x => x.Key, otherApprenticeshipKey2)
                    .With(x => x.PriceHistories, new List<PriceHistory>() { new() { ApprenticeshipKey = otherApprenticeshipKey2}})
                    .Create()
            };

            await _dbContext.AddRangeAsync(apprenticeships);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _sut.GetPendingPriceChange(apprenticeshipKey);

            // Assert
            result.Should().BeNull();
        }

        [TestCase("Employer")]
        [TestCase("Provider")]
        public async Task ThenTheCorrectPendingPriceChangeIsReturned(string initiator)
        {
            //Arrange
            SetUpApprenticeshipQueryRepository(_fixture.Create<long>());

            //Act
            var apprenticeshipKey = _fixture.Create<Guid>();
            var otherApprenticeshipKey = _fixture.Create<Guid>();
            var priceHistoryKey = _fixture.Create<Guid>();
            var effectiveFromDate = DateTime.UtcNow.AddDays(-5).Date;
            var providerApprovedDate = initiator == "Provider" ? _fixture.Create<DateTime>() : (DateTime?)null;
            var employerApprovedDate = initiator == "Employer" ? _fixture.Create<DateTime>() : (DateTime?)null;

            var apprenticeships = new[]
            {
                _fixture.Build<DataAccess.Entities.Apprenticeship.Apprenticeship>()
                    .With(x => x.Key, apprenticeshipKey)
                    .With(x => x.PriceHistories, new List<PriceHistory>() { new()
                        {
                            Key = priceHistoryKey,
                            ApprenticeshipKey = apprenticeshipKey,
                            PriceChangeRequestStatus = PriceChangeRequestStatus.Created,
                            TrainingPrice = 10000,
                            AssessmentPrice = 3000,
                            TotalPrice = 13000,
                            EffectiveFromDate = effectiveFromDate,
                            ChangeReason = "testReason",
                            ProviderApprovedDate = providerApprovedDate,
                            EmployerApprovedDate = employerApprovedDate,
                            ProviderApprovedBy = initiator == "Provider" ? "Mr Provider" : null,
                            EmployerApprovedBy = initiator == "Employer" ? "Mr Employer" : null,
                            Initiator = initiator == "Employer" ? PriceChangeInitiator.Employer : PriceChangeInitiator.Provider
                        }
                    })
                    .Create(), 
                _fixture.Build<DataAccess.Entities.Apprenticeship.Apprenticeship>()
                    .With(x => x.Key, otherApprenticeshipKey)
                    .With(x => x.PriceHistories, new List<PriceHistory>() { new() { ApprenticeshipKey = otherApprenticeshipKey}})
                    .Create()
            };

            await _dbContext.AddRangeAsync(apprenticeships);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _sut.GetPendingPriceChange(apprenticeshipKey);

            // Assert
            result.Should().NotBeNull();
            result.OriginalTrainingPrice.Should().Be(apprenticeships[0].TrainingPrice);
            result.OriginalAssessmentPrice.Should().Be(apprenticeships[0].EndPointAssessmentPrice);
            result.OriginalTotalPrice.Should().Be(apprenticeships[0].TotalPrice);
            result.PendingTrainingPrice.Should().Be(10000);
            result.PendingAssessmentPrice.Should().Be(3000);
            result.PendingTotalPrice.Should().Be(13000);
            result.EffectiveFrom.Should().Be(effectiveFromDate);
            result.Reason.Should().Be("testReason");
            result.Ukprn.Should().Be(apprenticeships[0].Ukprn);
            result.ProviderApprovedDate.Should().Be(providerApprovedDate);
            result.EmployerApprovedDate.Should().Be(employerApprovedDate);
            result.AccountLegalEntityId.Should().Be(apprenticeships[0].AccountLegalEntityId);
            result.Initiator.Should().Be(initiator);
        }

        private void SetUpApprenticeshipQueryRepository(long ukprn)
        {
            _accountIdClaimsHandler = AuthorizationHelper.MockAccountIdClaimsHandler(ukprn, AccountIdClaimsType.Provider);
            _dbContext = InMemoryDbContextCreator.SetUpInMemoryDbContext();
            var logger = Mock.Of<ILogger<Domain.Repositories.ApprenticeshipQueryRepository>>();
            _sut = new Domain.Repositories.ApprenticeshipQueryRepository(new Lazy<ApprenticeshipsDataContext>(_dbContext), logger);
        }
    }
}
