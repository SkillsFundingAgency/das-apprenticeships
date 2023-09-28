using AutoFixture;
using Dapper;
using Microsoft.Data.SqlClient;
using NServiceBus;
using SFA.DAS.Apprenticeships.AcceptanceTests.Handlers;
using SFA.DAS.Apprenticeships.AcceptanceTests.Helpers;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Events;
using SFA.DAS.Apprenticeships.Types;
using SFA.DAS.Approvals.EventHandlers.Messages;

namespace SFA.DAS.Apprenticeships.AcceptanceTests.StepDefinitions
{
    [Binding]
    [Scope(Feature = "PriceChangeApproved")]
    public class PriceChangeApprovedStepDefinitions
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly TestContext _testContext;
        private readonly Fixture _fixture;

        public PriceChangeApprovedStepDefinitions(ScenarioContext scenarioContext, TestContext testContext)
        {
            _scenarioContext = scenarioContext;
            _testContext = testContext;
            _fixture = new Fixture();
        }

        [Given(@"An existing apprenticeship")]
        public async Task GivenAnExistingApprenticeship()
        {
            var approvalCreatedEvent = _fixture.Build<ApprovalCreatedEvent>()
                .With(_ => _.Uln, _fixture.Create<long>().ToString)
                .With(_ => _.TrainingCode, _fixture.Create<int>().ToString)
                .Create();

            await _testContext.EndpointInstance.Publish(approvalCreatedEvent);

            await WaitHelper.WaitForIt(() => ApprenticeshipCreatedEventHandler.ReceivedEvents.Any(e => e.ApprovalsApprenticeshipId == approvalCreatedEvent.ApprovalsApprenticeshipId), $"Failed to find published {nameof(ApprenticeshipCreated)} event");

            _scenarioContext[nameof(ApprenticeshipCreatedEvent)] = ApprenticeshipCreatedEventHandler.ReceivedEvents.Single(e => e.Uln == approvalCreatedEvent.Uln);
        }

        [Given(@"A the apprenticeship's price change initiated by Provider")]
        public void GivenATheApprenticeshipsPriceChangeInitiatedByProvider()
        {
            // blank
        }

        [When(@"the price change is approved by Employer")]
        public async Task WhenThePriceChangeIsApprovedByEmployer()
        {
            var @event = new PriceChangeApprovedByEmployer
            {
                ApprenticeshipId = Apprenticeship.ApprovalsApprenticeshipId,
                TrainingPrice = 555,
                AssessmentPrice = 222,
                ApprovedDate = DateTime.Now.AddMinutes(-1),
                EffectiveFrom = DateTime.Now.AddMonths(3),
                EmployerAccountId = Apprenticeship.EmployerAccountId
            }; 
            
           await _testContext.EndpointInstance.Publish(@event);

           _scenarioContext[nameof(PriceChangeApprovedByEmployer)] = @event;
        }


        [Then(@"the price change is recorded against the Apprenticeship")]
        public void ThenThePriceChangeIsRecordedAgainstTheApprenticeship()
        {
            // blank
        }

        [Then(@"the price change is recorded in the database")]
        public async Task ThenThePriceChangeIsRecordedInTheDatabase()
        {
            await using var dbConnection = new SqlConnection(_testContext.SqlDatabase?.DatabaseInfo.ConnectionString);
            await WaitHelper.WaitForIt(async () => await ApprenticeshipRecordMatchesExpectation(dbConnection), "Failed to find apprenticeship price history record");

            var query = $"select * from PriceHistory where ApprenticeshipKey='{Apprenticeship.ApprenticeshipKey}'";
            var priceHistory = await dbConnection.QuerySingleAsync<PriceHistory>(query);

            priceHistory.ApprovedDate.Should().BeCloseTo(PriceChangeApprovedByEmployer.ApprovedDate,TimeSpan.FromSeconds(1));
            priceHistory.AssessmentPrice.Should().Be(PriceChangeApprovedByEmployer.AssessmentPrice);
            priceHistory.TrainingPrice.Should().Be(PriceChangeApprovedByEmployer.TrainingPrice);
            priceHistory.TotalPrice.Should().Be(PriceChangeApprovedByEmployer.TrainingPrice + PriceChangeApprovedByEmployer.AssessmentPrice);
            priceHistory.EffectiveFrom.Should().BeCloseTo(PriceChangeApprovedByEmployer.EffectiveFrom, TimeSpan.FromSeconds(1));
        }

        [Then(@"an ApprenticeshipPriceChanged event is published")]
        public async Task ThenAnApprenticeshipPriceChangedEventIsPublished()
        {
            await WaitHelper.WaitForIt(() => ApprenticeshipCreatedEventHandler.ReceivedEvents.Any(e => e.ApprenticeshipKey == Apprenticeship.ApprenticeshipKey), $"Failed to find published {nameof(ApprenticeshipPriceChangedEvent)} event");

            var sentEvent = ApprenticeshipPriceChangedEventHandler.ReceivedEvents.First();
            sentEvent.AssessmentPrice.Should().Be(PriceChangeApprovedByEmployer.AssessmentPrice);
            sentEvent.ApprovedDate.Should().Be(PriceChangeApprovedByEmployer.ApprovedDate);
            sentEvent.EffectiveFrom.Should().Be(PriceChangeApprovedByEmployer.EffectiveFrom);
            sentEvent.TrainingPrice.Should().Be(PriceChangeApprovedByEmployer.TrainingPrice);
            sentEvent.TotalPrice.Should().Be(PriceChangeApprovedByEmployer.AssessmentPrice + PriceChangeApprovedByEmployer.TrainingPrice);
        }

        private async Task<bool> ApprenticeshipRecordMatchesExpectation(SqlConnection dbConnection)
        {
            var query = $"select count(1) from PriceHistory where ApprenticeshipKey='{Apprenticeship.ApprenticeshipKey}'";
            var count = await dbConnection.QuerySingleAsync<int>(query);
            return count == 1;
        }

        protected ApprenticeshipCreatedEvent Apprenticeship => (ApprenticeshipCreatedEvent)_scenarioContext[nameof(ApprenticeshipCreatedEvent)];
        protected PriceChangeApprovedByEmployer PriceChangeApprovedByEmployer => (PriceChangeApprovedByEmployer)_scenarioContext[nameof(PriceChangeApprovedByEmployer)];
    }
}
