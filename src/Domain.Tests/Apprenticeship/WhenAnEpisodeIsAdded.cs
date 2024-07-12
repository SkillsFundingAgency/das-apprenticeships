using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Apprenticeship
{
    [TestFixture]
    public class WhenAnEpisodeIsAdded
    {
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _fixture.Customize(new ApprenticeshipCustomization());
        }

        [Test]
        public void ThenAnEpisodeAndPriceIsAdded()
        {
            var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
            var episodePrice = _fixture.Build<EpisodePrice>().With(x => x.IsDeleted, false).Create();
            var episode = EpisodeDomainModel.Get(_fixture.Build<Episode>()
                .With(x => x.Prices, new List<EpisodePrice>(){ episodePrice })
                .With(x => x.PaymentsFrozen, false)
                .Create());

            apprenticeship.AddEpisode(
                episode.Ukprn,
                episode.EmployerAccountId,
                episodePrice.StartDate,
                episodePrice.EndDate,
                episodePrice.TotalPrice,
                episodePrice.TrainingPrice,
                episodePrice.EndPointAssessmentPrice,
                episode.FundingType,
                episode.FundingPlatform,
                episodePrice.FundingBandMaximum,
                episode.FundingEmployerAccountId,
                episode.LegalEntityName,
                episode.AccountLegalEntityId,
                episode.TrainingCode,
                episode.TrainingCourseVersion);

            apprenticeship.LatestEpisode.Should().BeEquivalentTo(episode);
        }
    }
}
