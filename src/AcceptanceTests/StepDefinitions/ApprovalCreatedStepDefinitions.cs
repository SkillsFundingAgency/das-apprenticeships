using AutoFixture;
using Dapper.Contrib.Extensions;
using Microsoft.Data.SqlClient;
using Moq;
using SFA.DAS.Learning.AcceptanceTests.Helpers;
using SFA.DAS.Learning.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Learning.Infrastructure.ApprenticeshipsOuterApiClient.Standards;
using SFA.DAS.Learning.Types;
using FundingPlatform = SFA.DAS.Learning.Enums.FundingPlatform;

namespace SFA.DAS.Learning.AcceptanceTests.StepDefinitions
{
    [Binding]
    public class ApprovalCreatedStepDefinitions
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly TestContext _testContext;
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
            var approvalCreatedEvent = _fixture.Build<CommitmentsV2.Messages.Events.ApprenticeshipCreatedEvent> ()
                .With(_ => _.TrainingCourseVersion, "1.0")
                .With(_ => _.IsOnFlexiPaymentPilot, true)
                .With(_ => _.Uln, _fixture.Create<long>().ToString)
                .With(_ => _.TrainingCode, _fixture.Create<int>().ToString)
                .Create();

            await _testContext.TestFunction.PublishEvent(approvalCreatedEvent);

            _scenarioContext["ApprovalCreatedEvent"] = approvalCreatedEvent;
        }

        [Given("the funding band maximum for that apprenticeship is set")]
        public void GivenTheFundingBandMaximumForThatApprenticeshipIsSet()
        {
            var fundingBandMaximum = _fixture.Create<int>();
            _scenarioContext["fundingBandMaximum"] = fundingBandMaximum;

            _testContext.TestFunction.mockApprenticeshipsOuterApiClient.Reset();
            _testContext.TestFunction.mockApprenticeshipsOuterApiClient
                .Setup(x => x.GetStandard(It.IsAny<int>()))
                .ReturnsAsync(new GetStandardResponse { MaxFunding = fundingBandMaximum, ApprenticeshipFunding = 
                    new List<GetStandardFundingResponse>
                    {
                        new() { EffectiveFrom = DateTime.MinValue, EffectiveTo = null, MaxEmployerLevyCap = fundingBandMaximum }
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
                    new() { EffectiveFrom = DateTime.MinValue, EffectiveTo = DateTime.MinValue, MaxEmployerLevyCap = fundingBandMaximum }
                }
            });
        }

        [Then(@"an Apprenticeship record is created")]
        public async Task ThenAnApprenticeshipRecordIsCreated()
        {
            await WaitHelper.WaitForIt(async () => await ApprenticeshipRecordMatchesExpectation(), "Failed to find the apprenticeship record");

            await using var dbConnection = new SqlConnection(_testContext.SqlDatabase?.DatabaseInfo.ConnectionString);

            var apprenticeship = dbConnection.GetAll<Apprenticeship>().Single(x => x.Uln == ApprovalCreatedEvent.Uln);
            apprenticeship.Should().NotBeNull();
            apprenticeship.Uln.Should().Be(ApprovalCreatedEvent.Uln);
            apprenticeship.FirstName.Should().Be(ApprovalCreatedEvent.FirstName);
            apprenticeship.LastName.Should().Be(ApprovalCreatedEvent.LastName);
            apprenticeship.Key.Should().NotBe(Guid.Empty);
            apprenticeship.ApprovalsApprenticeshipId.Should().Be(ApprovalCreatedEvent.ApprenticeshipId);
           
            var episode = (await dbConnection.GetAllAsync<Episode>()).Last(x => x.ApprenticeshipKey == apprenticeship.Key);
            episode.Should().NotBeNull();
            episode.Key.Should().NotBe(Guid.Empty);
            episode.Ukprn.Should().Be(ApprovalCreatedEvent.ProviderId);
            episode.EmployerAccountId.Should().Be(ApprovalCreatedEvent.AccountId);
            episode.FundingEmployerAccountId.Should().Be(ApprovalCreatedEvent.TransferSenderId);
            episode.LegalEntityName.Should().Be(ApprovalCreatedEvent.LegalEntityName);
            episode.FundingPlatform.Should().Be(ApprovalCreatedEvent.IsOnFlexiPaymentPilot.HasValue ? (ApprovalCreatedEvent.IsOnFlexiPaymentPilot.Value ? FundingPlatform.DAS : FundingPlatform.SLD) : null);
            int.Parse(episode.TrainingCode).Should().Be(int.Parse(ApprovalCreatedEvent.TrainingCode));

            var episodePrice = (await dbConnection.GetAllAsync<EpisodePrice>()).Last(x => x.EpisodeKey == episode.Key);
            episodePrice.Should().NotBeNull();
            episodePrice.StartDate.Should().BeSameDateAs(ApprovalCreatedEvent.ActualStartDate!.Value);
            episodePrice.EndDate.Should().BeSameDateAs(ApprovalCreatedEvent.EndDate);
            episodePrice.TotalPrice.Should().Be(ApprovalCreatedEvent.PriceEpisodes[0].Cost);

            _scenarioContext["Learning"] = apprenticeship;
            _scenarioContext["Episode"] = episode;
            _scenarioContext["EpisodePrice"] = episodePrice;
        }

        [Then("an Apprenticeship record is created with the correct funding band maximum")]
        public async Task ThenAnApprenticeshipRecordIsCreatedWithTheCorrectFundingBandMaximum()
        {
            await ThenAnApprenticeshipRecordIsCreated();
            ((EpisodePrice)_scenarioContext["EpisodePrice"]).FundingBandMaximum.Should().Be((int)_scenarioContext["fundingBandMaximum"]);
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
            await WaitHelper.WaitForIt(() => _testContext.MessageSession.ReceivedEvents<ApprenticeshipCreatedEvent>().Any(EventMatchesExpectation), $"Failed to find published {nameof(ApprenticeshipCreatedEvent)} event");

            var publishedEvent = _testContext.MessageSession.ReceivedEvents<ApprenticeshipCreatedEvent>().Single(EventMatchesExpectation);

            publishedEvent.Uln.Should().Be(Apprenticeship.Uln);
            publishedEvent.ApprenticeshipKey.Should().Be(Apprenticeship.Key);
            int.Parse(publishedEvent.Episode.TrainingCode).Should().Be(int.Parse(LatestEpisode.TrainingCode));
            publishedEvent.Episode.Prices.MaxBy(x => x.StartDate)?.StartDate.Should().BeSameDateAs(LatestEpisodePrice.StartDate);
            publishedEvent.Episode.Prices.MaxBy(x => x.StartDate)?.EndDate.Should().BeSameDateAs(LatestEpisodePrice.EndDate);
            publishedEvent.Episode.Prices.MaxBy(x => x.StartDate)?.TotalPrice.Should().Be(LatestEpisodePrice.TotalPrice);
            publishedEvent.ApprovalsApprenticeshipId.Should().Be(Apprenticeship.ApprovalsApprenticeshipId);
            publishedEvent.Episode.EmployerAccountId.Should().Be(LatestEpisode.EmployerAccountId);
            publishedEvent.Episode.FundingEmployerAccountId.Should().Be(LatestEpisode.FundingEmployerAccountId);
            publishedEvent.Episode.FundingType.ToString().Should().Be(LatestEpisode.FundingType.ToString());
            publishedEvent.Episode.LegalEntityName.Should().Be(LatestEpisode.LegalEntityName);
            publishedEvent.Episode.Ukprn.Should().Be(LatestEpisode.Ukprn);
            publishedEvent.FirstName.Should().Be(Apprenticeship.FirstName);
            publishedEvent.LastName.Should().Be(Apprenticeship.LastName);
            publishedEvent.Episode.FundingPlatform.ToString().Should().Be(LatestEpisode.FundingPlatform.ToString());

            _scenarioContext["publishedEvent"] = publishedEvent;
        }

        [Then("an ApprenticeshipCreatedEvent event is published with the correct funding band maximum")]
        public async Task ThenAnApprenticeshipCreatedEventEventIsPublishedWithTheCorrectFundingBandMaximum()
        {
            await ThenAnApprenticeshipCreatedEventEventIsPublished();
            ((ApprenticeshipCreatedEvent)_scenarioContext["publishedEvent"])
                .Episode
                .Prices
                .MaxBy(x => x.StartDate)?
                .FundingBandMaximum
                .Should().Be((int)_scenarioContext["fundingBandMaximum"]);
        }

        [Then(@"an ApprenticeshipCreatedEvent event is not published")]
        public async Task ThenAnApprenticeshipCreatedEventEventIsNotPublished()
        {
            await WaitHelper.WaitForUnexpected(() => _testContext.MessageSession.ReceivedEvents<ApprenticeshipCreatedEvent>().Any(EventMatchesExpectation), $"Found unexpected {nameof(ApprenticeshipCreatedEvent)} event");
        }


        private bool EventMatchesExpectation(ApprenticeshipCreatedEvent apprenticeshipCreatedEvent)
        {
            return apprenticeshipCreatedEvent.Uln == ApprovalCreatedEvent.Uln;
        }

        public CommitmentsV2.Messages.Events.ApprenticeshipCreatedEvent ApprovalCreatedEvent => (CommitmentsV2.Messages.Events.ApprenticeshipCreatedEvent)_scenarioContext["ApprovalCreatedEvent"];
        public Apprenticeship Apprenticeship => (Apprenticeship)_scenarioContext["Learning"];
        public Episode LatestEpisode => (Episode)_scenarioContext["Episode"];
        public EpisodePrice LatestEpisodePrice => (EpisodePrice)_scenarioContext["EpisodePrice"];
    }
}
