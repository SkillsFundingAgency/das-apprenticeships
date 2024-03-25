using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.DataAccess;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Events;
using SFA.DAS.Apprenticeships.Domain.Factories;
using SFA.DAS.Apprenticeships.TestHelpers;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Repositories.ApprenticeshipRepository
{
    public class WhenAddingAnApprenticeship
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
        public async Task Then_the_apprenticeship_is_added_to_the_data_store()
        {
            // Arrange
            var testApprenticeship = ApprenticeshipDomainModel.Get(_fixture.Create<DataAccess.Entities.Apprenticeship.Apprenticeship>());
            SetUpApprenticeshipRepository(testApprenticeship);

            // Act
            await _sut.Add(testApprenticeship);
            
            // Assert
            _dbContext.Apprenticeships.Count().Should().Be(1);

            var storedApprenticeship = _dbContext.Apprenticeships.Include(x => x.Approvals).Include(x => x.PriceHistories).Single();
            var expectedModel = testApprenticeship.GetEntity();

            expectedModel.Should().BeEquivalentTo(storedApprenticeship);
        }

        [Test]
        public async Task Then_the_approval_is_added_to_the_data_store()
        {
            // Arrange
            var apprenticeshipEntity = _fixture.Create<DataAccess.Entities.Apprenticeship.Apprenticeship>();
            apprenticeshipEntity.Approvals = new List<Approval>();
            var testApprenticeship = ApprenticeshipDomainModel.Get(apprenticeshipEntity);
            SetUpApprenticeshipRepository(testApprenticeship);
            var expectedApproval = ApprovalDomainModel.Get(_fixture.Create<Approval>());

            // Act
            testApprenticeship.AddApproval(
                expectedApproval.ApprovalsApprenticeshipId, 
                expectedApproval.LegalEntityName, 
                expectedApproval.ActualStartDate, 
                expectedApproval.PlannedEndDate, 
                expectedApproval.AgreedPrice, 
                expectedApproval.FundingEmployerAccountId, 
                expectedApproval.FundingType, 
                expectedApproval.FundingBandMaximum, 
                expectedApproval.PlannedStartDate, 
                expectedApproval.FundingPlatform);
            await _sut.Add(testApprenticeship);
            
            // Assert
            _dbContext.Approvals.Count().Should().Be(1);
            var storedApproval = _dbContext.Approvals.Single();
            storedApproval.Should().BeEquivalentTo(expectedApproval);
        }

        [Test]
        public async Task Then_the_domain_events_are_published()
        {
            // Arrange
            var testApprenticeship = ApprenticeshipDomainModel.New(
                "1234435",                   //  string uln,
                "TRN",                       //  string trainingCode,
                new DateTime(2000, 10, 16),  //  DateTime dateOfBirth,
                "Ron",                       //  string firstName,
                "Swanson",                   //  string lastName,
                _fixture.Create<decimal?>(), //  decimal? trainingPrice,
                _fixture.Create<decimal?>(), //  decimal? endpointAssessmentPrice,
                _fixture.Create<decimal>(),  //  decimal totalPrice,
                _fixture.Create<string>(),   //  string apprenticeshipHashedId,
                _fixture.Create<int>(),      //  int fundingBandMaximum,
                _fixture.Create<DateTime>(), //  DateTime? actualStartDate,
                _fixture.Create<DateTime>(), //  DateTime? plannedEndDate,
                _fixture.Create<long>(),     //  long accountLegalEntityId,
				_fixture.Create<long>(),     //  long ukprn,
				_fixture.Create<long>());    //  long employerAccountId
            SetUpApprenticeshipRepository(testApprenticeship);
			await _sut.Add(testApprenticeship);
            
            // Assert
            _domainEventDispatcher.Verify(x => x.Send(It.Is<ApprenticeshipCreated>(e => e.ApprenticeshipKey == testApprenticeship.Key), It.IsAny<CancellationToken>()), Times.Once());
        }

        private void SetUpApprenticeshipRepository(ApprenticeshipDomainModel testApprenticeship)
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
