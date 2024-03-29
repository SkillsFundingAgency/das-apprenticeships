using AutoFixture;
using Dapper.Contrib.Extensions;
using Microsoft.Data.SqlClient;
using Moq;
using NServiceBus;
using SFA.DAS.Apprenticeships.AcceptanceTests.Handlers;
using SFA.DAS.Apprenticeships.AcceptanceTests.Helpers;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Events;
using SFA.DAS.Apprenticeships.Functions;
using SFA.DAS.Apprenticeships.Infrastructure.ApprenticeshipsOuterApiClient;
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
            var approvalCreatedEvent = _fixture.Build<ApprovalCreatedEvent>() 
                .With(_ => _.Uln, _fixture.Create<long>().ToString)
                .With(_ => _.TrainingCode, _fixture.Create<int>().ToString)
                .Create();

            await _endpointInstance.Publish(approvalCreatedEvent);

            _scenarioContext["ApprovalCreatedEvent"] = approvalCreatedEvent;
        }

        [Given("the funding band maximum for that apprenticeship is set")]
        public void GivenTheFundingBandMaximumForThatApprenticeshipIsSet()
        {
            var fundingBandMaximum = _fixture.Create<int>();
            _scenarioContext["fundingBandMaximum"] = fundingBandMaximum;
            _testContext.TestFunction.mockApprenticeshipsOuterApiClient.Setup(x => x.GetStandard(It.IsAny<int>())).ReturnsAsync(new GetStandardResponse { MaxFunding = fundingBandMaximum, ApprenticeshipFunding = new List<GetStandardFundingResponse>
            {
                new GetStandardFundingResponse{ EffectiveFrom = DateTime.MinValue, EffectiveTo = null, MaxEmployerLevyCap = fundingBandMaximum }
            }});
        }

        [Given("a funding band maximum for that apprenticeship and date range is not available")]
        public void GivenAFundingBandMaximumForThatApprenticeshipAndDateRangeIsNotAvailable()
        {
            var fundingBandMaximum = _fixture.Create<int>();
            _scenarioContext["fundingBandMaximum"] = fundingBandMaximum;
            _testContext.TestFunction.mockApprenticeshipsOuterApiClient.Setup(x => x.GetStandard(It.IsAny<int>())).ReturnsAsync(new GetStandardResponse
            {
                MaxFunding = fundingBandMaximum,
                ApprenticeshipFunding = new List<GetStandardFundingResponse>
                {
                    new GetStandardFundingResponse{ EffectiveFrom = DateTime.MinValue, EffectiveTo = DateTime.MinValue, MaxEmployerLevyCap = fundingBandMaximum }
                }
            });
        }


        [Then(@"an Apprenticeship record is created")]
        public async Task ThenAnApprenticeshipRecordIsCreated()
        {
            await WaitHelper.WaitForIt(async () => await ApprenticeshipRecordMatchesExpectation(), "Failed to find the apprenticeship record");

            await using var dbConnection = new SqlConnection(_testContext.SqlDatabase?.DatabaseInfo.ConnectionString);

            var apprenticeship = dbConnection.GetAll<Apprenticeship>().Single(x => x.Uln == ApprovalCreatedEvent.Uln);
            apprenticeship.Uln.Should().Be(ApprovalCreatedEvent.Uln);
            apprenticeship.FirstName.Should().Be(ApprovalCreatedEvent.FirstName);
            apprenticeship.LastName.Should().Be(ApprovalCreatedEvent.LastName);
            apprenticeship.EmployerAccountId.Should().Be(ApprovalCreatedEvent.EmployerAccountId);
            apprenticeship.Ukprn.Should().Be(ApprovalCreatedEvent.UKPRN);
            int.Parse(apprenticeship.TrainingCode).Should().Be(int.Parse(ApprovalCreatedEvent.TrainingCode));
            apprenticeship.Key.Should().NotBe(Guid.Empty);
           
            var approval = (await dbConnection.GetAllAsync<Approval>()).Single(x => x.ApprenticeshipKey == apprenticeship.Key);
            approval.ActualStartDate.Should().BeSameDateAs(ApprovalCreatedEvent.ActualStartDate!.Value);
            approval.ApprovalsApprenticeshipId.Should().Be(ApprovalCreatedEvent.ApprovalsApprenticeshipId);
            approval.AgreedPrice.Should().Be(ApprovalCreatedEvent.AgreedPrice);
            approval.FundingEmployerAccountId.Should().Be(ApprovalCreatedEvent.FundingEmployerAccountId);
            approval.FundingType.Should().Be(Enum.Parse<Enums.FundingType>(ApprovalCreatedEvent.FundingType.ToString()));
            approval.LegalEntityName.Should().Be(ApprovalCreatedEvent.LegalEntityName);
            approval.PlannedEndDate.Should().BeSameDateAs(ApprovalCreatedEvent.PlannedEndDate!.Value);
            approval.Id.Should().NotBe(Guid.Empty);
            approval.PlannedStartDate.Should().BeSameDateAs(ApprovalCreatedEvent.StartDate!.Value);
            approval.FundingPlatform.Should().Be(ApprovalCreatedEvent.IsOnFlexiPaymentPilot.HasValue ? (ApprovalCreatedEvent.IsOnFlexiPaymentPilot.Value ? Enums.FundingPlatform.DAS : Enums.FundingPlatform.SLD) : null);

            _scenarioContext["Apprenticeship"] = apprenticeship;
            _scenarioContext["Approval"] = approval;
        }

        [Then("an Apprenticeship record is created with the correct funding band maximum")]
        public async Task ThenAnApprenticeshipRecordIsCreatedWithTheCorrectFundingBandMaximum()
        {
            await ThenAnApprenticeshipRecordIsCreated();
            ((Approval)_scenarioContext["Approval"]).FundingBandMaximum.Should().Be((int)_scenarioContext["fundingBandMaximum"]);
        }

        [Then("an Apprenticeship record is not created")]
        public async Task ThenAnApprenticeshipRecordIsNotCreated()
        {
            await WaitHelper.WaitForUnexpected(ApprenticeshipRecordMatchesExpectation, "Found unexpected apprenticeship record created.");
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

            var publishedEvent = ApprenticeshipCreatedEventHandler.ReceivedEvents.Single(EventMatchesExpectation);

            publishedEvent.Uln.Should().Be(Apprenticeship.Uln);
            publishedEvent.ApprenticeshipKey.Should().Be(Apprenticeship.Key);
            int.Parse(publishedEvent.TrainingCode).Should().Be(int.Parse(Apprenticeship.TrainingCode));
            publishedEvent.ActualStartDate.Should().BeSameDateAs(Approval.ActualStartDate ?? DateTime.Now);
            publishedEvent.PlannedEndDate.Should().BeSameDateAs(Approval.PlannedEndDate);
            publishedEvent.AgreedPrice.Should().Be(Approval.AgreedPrice);
            publishedEvent.ApprovalsApprenticeshipId.Should().Be(Approval.ApprovalsApprenticeshipId);
            publishedEvent.EmployerAccountId.Should().Be(Apprenticeship.EmployerAccountId);
            publishedEvent.FundingEmployerAccountId.Should().Be(Approval.FundingEmployerAccountId);
            publishedEvent.FundingType.ToString().Should().Be(Approval.FundingType.ToString());
            publishedEvent.LegalEntityName.Should().Be(Approval.LegalEntityName);
            publishedEvent.UKPRN.Should().Be(Apprenticeship.Ukprn);
            publishedEvent.FirstName.Should().Be(Apprenticeship.FirstName);
            publishedEvent.LastName.Should().Be(Apprenticeship.LastName);
            publishedEvent.PlannedStartDate.Should().BeSameDateAs(Approval.PlannedStartDate!.Value);
            publishedEvent.FundingPlatform.ToString().Should().Be(Approval.FundingPlatform.ToString());

            _scenarioContext["publishedEvent"] = publishedEvent;
        }

        [Then("an ApprenticeshipCreatedEvent event is published with the correct funding band maximum")]
        public async Task ThenAnApprenticeshipCreatedEventEventIsPublishedWithTheCorrectFundingBandMaximum()
        {
            await ThenAnApprenticeshipCreatedEventEventIsPublished();
            ((ApprenticeshipCreatedEvent)_scenarioContext["publishedEvent"]).FundingBandMaximum.Should().Be((int)_scenarioContext["fundingBandMaximum"]);
        }

        [Then(@"an ApprenticeshipCreatedEvent event is not published")]
        public async Task ThenAnApprenticeshipCreatedEventEventIsNotPublished()
        {
            await WaitHelper.WaitForUnexpected(() => ApprenticeshipCreatedEventHandler.ReceivedEvents.Any(EventMatchesExpectation), $"Found unexpected {nameof(ApprenticeshipCreated)} event");
        }


        private bool EventMatchesExpectation(ApprenticeshipCreatedEvent apprenticeshipCreatedEvent)
        {
            return apprenticeshipCreatedEvent.Uln == ApprovalCreatedEvent.Uln;
        }

        public ApprovalCreatedEvent ApprovalCreatedEvent => (ApprovalCreatedEvent)_scenarioContext["ApprovalCreatedEvent"];
        public Apprenticeship Apprenticeship => (Apprenticeship)_scenarioContext["Apprenticeship"];
        public Approval Approval => (Approval)_scenarioContext["Approval"];
    }
}
