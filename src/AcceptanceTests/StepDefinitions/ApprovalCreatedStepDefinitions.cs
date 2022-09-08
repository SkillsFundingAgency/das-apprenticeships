using AutoFixture;
using Dapper.Contrib.Extensions;
using Microsoft.Data.SqlClient;
using NServiceBus;
using SFA.DAS.Apprenticeships.AcceptanceTests.Handlers;
using SFA.DAS.Apprenticeships.AcceptanceTests.Helpers;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Events;
using SFA.DAS.Apprenticeships.Infrastructure;
using SFA.DAS.Apprenticeships.Types;
using SFA.DAS.Approvals.EventHandlers.Messages;

namespace SFA.DAS.Apprenticeships.AcceptanceTests.StepDefinitions
{
    [Binding]
    public class ApprovalCreatedStepDefinitions
    {
        private static IEndpointInstance? _endpointInstance;
        private readonly ScenarioContext _scenarioContext;
        private readonly TestContext _testContext;
        private readonly Fixture _fixture;

        public ApprovalCreatedStepDefinitions(ScenarioContext scenarioContext, TestContext testContext)
        {
            _scenarioContext = scenarioContext;
            _testContext = testContext;
            _fixture = new Fixture();
        }

        [BeforeTestRun]
        public static async Task StartEndpoint()
        {
            _endpointInstance = await EndpointHelper
                .StartEndpoint(QueueNames.ApprovalCreated + "TEST", false, new[] { typeof(ApprovalCreatedEvent), typeof(ApprenticeshipCreatedEvent) });
        }

        [AfterTestRun]
        public static async Task StopEndpoint()
        {
            await _endpointInstance!.Stop()
                .ConfigureAwait(false);
        }

        [Given(@"An apprenticeship has been created as part of the approvals journey")]
        public async Task GivenAnApprenticeshipHasBeenCreatedAsPartOfTheApprovalsJourney()
        {
            var ApprovalCreatedEvent = _fixture.Build<ApprovalCreatedEvent>() 
                .With(_ => _.Uln, _fixture.Create<long>().ToString)
                .With(_ => _.TrainingCode, _fixture.Create<string>()[..10])
                .Create();

            await _endpointInstance.Publish(ApprovalCreatedEvent);

            _scenarioContext["ApprovalCreatedEvent"] = ApprovalCreatedEvent;
        }

        [Then(@"an Apprenticeship record is created")]
        public async Task ThenAnApprenticeshipRecordIsCreated()
        {
            await WaitHelper.WaitForIt(async () => await ApprenticeshipRecordMatchesExpectation(), "Failed to find the apprenticeship record");

            await using var dbConnection = new SqlConnection(_testContext.SqlDatabase?.DatabaseInfo.ConnectionString);

            var apprenticeship = dbConnection.GetAll<Apprenticeship>().Single(x => x.Uln == ApprovalCreatedEvent.Uln);
            apprenticeship.Uln.Should().Be(ApprovalCreatedEvent.Uln);
            apprenticeship.TrainingCode.Should().Be(ApprovalCreatedEvent.TrainingCode);
            apprenticeship.Key.Should().NotBe(Guid.Empty);
           
            var approval = (await dbConnection.GetAllAsync<Approval>()).Single(x => x.ApprenticeshipKey == apprenticeship.Key);
            approval.ActualStartDate.Should().BeSameDateAs(ApprovalCreatedEvent.ActualStartDate.Value);
            approval.ApprovalsApprenticeshipId.Should().Be(ApprovalCreatedEvent.ApprovalsApprenticeshipId);
            approval.AgreedPrice.Should().Be(ApprovalCreatedEvent.AgreedPrice);
            approval.EmployerAccountId.Should().Be(ApprovalCreatedEvent.EmployerAccountId);
            approval.FundingEmployerAccountId.Should().Be(ApprovalCreatedEvent.FundingEmployerAccountId);
            approval.FundingType.Should().Be(Enum.Parse<Enums.FundingType>(ApprovalCreatedEvent.FundingType.ToString()));
            approval.LegalEntityName.Should().Be(ApprovalCreatedEvent.LegalEntityName);
            approval.PlannedEndDate.Should().BeSameDateAs(ApprovalCreatedEvent.PlannedEndDate.Value);
            approval.UKPRN.Should().Be(ApprovalCreatedEvent.UKPRN);
            approval.Id.Should().NotBe(Guid.Empty);

            _scenarioContext["Apprenticeship"] = apprenticeship;
            _scenarioContext["Approval"] = approval;
        }

        private async Task<bool> ApprenticeshipRecordMatchesExpectation()
        {
            await using var dbConnection = new SqlConnection(_testContext.SqlDatabase?.DatabaseInfo.ConnectionString);
            var apprenticeship = (await dbConnection.GetAllAsync<Apprenticeship>()).SingleOrDefault(x => x.Uln == ApprovalCreatedEvent.Uln);

            return apprenticeship != null;
        }

        [Then(@"an ApprenticeshipCreatedEvent event is published")]
        public async Task ThenAnApprenticeshipCreatedEventEventIsPublished()
        {
            await WaitHelper.WaitForIt(() => ApprenticeshipCreatedEventHandler.ReceivedEvents.Any(EventMatchesExpectation), $"Failed to find published {nameof(ApprenticeshipCreated)} event");

            var publishedEvent = ApprenticeshipCreatedEventHandler.ReceivedEvents.Single();

            publishedEvent.Uln.Should().Be(Apprenticeship.Uln);
            publishedEvent.ApprenticeshipKey.Should().Be(Apprenticeship.Key);
            publishedEvent.TrainingCode.Should().Be(Apprenticeship.TrainingCode);
            publishedEvent.ActualStartDate.Should().BeSameDateAs(Approval.ActualStartDate);
            publishedEvent.PlannedEndDate.Should().BeSameDateAs(Approval.PlannedEndDate);
            publishedEvent.AgreedPrice.Should().Be(Approval.AgreedPrice);
            publishedEvent.ApprovalsApprenticeshipId.Should().Be(Approval.ApprovalsApprenticeshipId);
            publishedEvent.EmployerAccountId.Should().Be(Approval.EmployerAccountId);
            publishedEvent.FundingEmployerAccountId.Should().Be(Approval.FundingEmployerAccountId);
            publishedEvent.FundingType.ToString().Should().Be(Approval.FundingType.ToString());
            publishedEvent.LegalEntityName.Should().Be(Approval.LegalEntityName);
            publishedEvent.UKPRN.Should().Be(Approval.UKPRN);
        }

        private bool EventMatchesExpectation(ApprenticeshipCreatedEvent apprenticeshipCreatedEvent)
        {
            return apprenticeshipCreatedEvent.ApprenticeshipKey == Apprenticeship.Key;
        }

        public ApprovalCreatedEvent ApprovalCreatedEvent => (ApprovalCreatedEvent)_scenarioContext["ApprovalCreatedEvent"];
        public Apprenticeship Apprenticeship => (Apprenticeship)_scenarioContext["Apprenticeship"];
        public Approval Approval => (Approval)_scenarioContext["Approval"];
    }
}
