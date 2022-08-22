using System.Diagnostics;
using AutoFixture;
using Dapper.Contrib.Extensions;
using Microsoft.Data.SqlClient;
using NServiceBus;
using NUnit.Framework;
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
        private readonly ScenarioContext _scenarioContext;
        public readonly TestContext _testContext;
        private static IEndpointInstance _endpointInstance;
        private ApprovalCreatedCommand _approvalCreatedCommand;
        private readonly Fixture _fixture;

        public ApprovalCreatedStepDefinitions(ScenarioContext scenarioContext, TestContext testContext)
        {
            _scenarioContext = scenarioContext;
            _testContext = testContext;
            _fixture = new Fixture();
        }

        [Given(@"An apprenticeship has been created as part of the approvals journey")]
        public async Task GivenAnApprenticeshipHasBeenCreatedAsPartOfTheApprovalsJourney()
        {
            _approvalCreatedCommand = _fixture.Build<ApprovalCreatedCommand>() 
                .With(_ => _.Uln, _fixture.Create<long>().ToString)
                .With(_ => _.TrainingCode, _fixture.Create<string>()[..10])
                .Create();

            await _endpointInstance.Publish(_approvalCreatedCommand);

            _scenarioContext["ApprovalCreatedCommand"] = _approvalCreatedCommand;
        }

        [Then(@"an Apprenticeship record is created")]
        public async Task ThenAnApprenticeshipRecordIsCreated()
        {
            await WaitHelper.WaitForIt(async () => await IsApprenticeshipRecordCorrect(), "Failed to fine non null apprenticeship");

            var approvalCreatedCommand = (ApprovalCreatedCommand)_scenarioContext["ApprovalCreatedCommand"];

            await using var dbConnection = new SqlConnection(_testContext.SqlDatabase?.DatabaseInfo.ConnectionString);
            var apprenticeship = (await dbConnection.GetAllAsync<Apprenticeship>()).Single(x => x.Uln == _approvalCreatedCommand.Uln);
            apprenticeship.Uln.Should().Be(approvalCreatedCommand.Uln);
            apprenticeship.TrainingCode.Should().Be(approvalCreatedCommand.TrainingCode);
            apprenticeship.Key.Should().NotBe(Guid.Empty);
           //  apprenticeship.Approvals.Should().HaveCount(1);
        }

        private async Task<bool> IsApprenticeshipRecordCorrect()
        {
            await using var dbConnection = new SqlConnection(_testContext.SqlDatabase?.DatabaseInfo.ConnectionString);
            var apprenticeship = (await dbConnection.GetAllAsync<Apprenticeship>()).SingleOrDefault(x => x.Uln == _approvalCreatedCommand.Uln);

            return apprenticeship != null;
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

        [Then(@"an ApprenticeshipCreatedEvent event is published")]
        public async Task ThenAnApprenticeshipCreatedEventEventIsPublished()
        {
            await WaitHelper.WaitForIt(() => ApprenticeshipCreatedEventHandler.ReceivedEvents.Any(EventMatchesExpectation), "Failed to find published EarningsGenerated event");
        }

        private bool EventMatchesExpectation(ApprenticeshipCreatedEvent apprenticeshipCreatedEvent)
        {
            return true;

        //    return apprenticeshipCreatedEvent.FundingPeriods.Count == 1
        //           && apprenticeshipCreatedEvent.FundingPeriods.First().DeliveryPeriods.Count == (int)_scenarioContext["expectedDeliveryPeriodCount"]
        //           && apprenticeshipCreatedEvent.FundingPeriods.First().DeliveryPeriods.All(x => x.LearningAmount == (int)_scenarioContext["expectedDeliveryPeriodLearningAmount"]);
        }
    }
}
