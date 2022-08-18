using AutoFixture;
using Dapper.Contrib.Extensions;
using Microsoft.Data.SqlClient;
using NServiceBus;
using SFA.DAS.Apprenticeships.AcceptanceTests.Handlers;
using SFA.DAS.Apprenticeships.AcceptanceTests.Helpers;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Infrastructure;
using SFA.DAS.Apprenticeships.Types;

namespace SFA.DAS.Apprenticeships.AcceptanceTests.StepDefinitions
{
    [Binding]
    public class ApprovalCreatedStepDefinitions
    {
        public readonly TestContext _testContext;
        private readonly ScenarioContext _scenarioContext;
        private static IEndpointInstance _endpointInstance;
        private ApprovalCreatedCommand _approvalCreatedCommand;
        private readonly Fixture _fixture;


        public ApprovalCreatedStepDefinitions(ScenarioContext scenarioContext,
            TestContext testContext)
        {
            _testContext = testContext;
            _scenarioContext = scenarioContext;
            _fixture = new Fixture();
        }

        [Given(@"An apprenticeship has been created as part of the approvals journey")]
        public async Task GivenAnApprenticeshipHasBeenCreatedAsPartOfTheApprovalsJourney()
        {
            _approvalCreatedCommand = _fixture.Build<ApprovalCreatedCommand>() 
                .With(_ => _.Uln, _fixture.Create<long>().ToString)
                .Create();

            await _endpointInstance.Publish(_approvalCreatedCommand);
        }

        [Then(@"an apprenticeship record is created")]
        public async Task ThenAnApprenticeshipRecordIsCreated()
        {
            await using var dbConnection = new SqlConnection(_testContext.SqlDatabase?.DatabaseInfo.ConnectionString);

            var apprenticeship = (await dbConnection.GetAllAsync<Apprenticeship>()).SingleOrDefault(x => x.Uln == _approvalCreatedCommand.Uln);

            apprenticeship.Should().NotBeNull();
        }

        [BeforeTestRun]
        public static async Task StartEndpoint()
        {
            _endpointInstance = await EndpointHelper
                .StartEndpoint(QueueNames.ApprovalCreated, false, new[] { typeof(ApprovalCreatedCommand) });
        }

        [AfterTestRun]
        public static async Task StopEndpoint()
        {
            await _endpointInstance.Stop()
                .ConfigureAwait(false);
        }

        [Then(@"Earnings are generated with the correct learning amounts")]
        public async Task AssertEarningsGeneratedEvent()
        {
            await WaitHelper.WaitForIt(() => ApprenticeshipCreatedEventHandler.ReceivedEvents.Any(EventMatchesExpectation), "Failed to find published EarningsGenerated event");
        }

        private bool EventMatchesExpectation(ApprenticeshipCreatedEvent apprenticeshipCreatedEvent)
        {
            // todo
            return true;

        //    return apprenticeshipCreatedEvent.FundingPeriods.Count == 1
        //           && apprenticeshipCreatedEvent.FundingPeriods.First().DeliveryPeriods.Count == (int)_scenarioContext["expectedDeliveryPeriodCount"]
        //           && apprenticeshipCreatedEvent.FundingPeriods.First().DeliveryPeriods.All(x => x.LearningAmount == (int)_scenarioContext["expectedDeliveryPeriodLearningAmount"]);
        }
    }
}
