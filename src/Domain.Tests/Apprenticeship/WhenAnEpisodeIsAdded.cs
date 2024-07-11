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

        //todo this should pass once freeze/unfreeze is sorted out
        [Test]
        public void ThenAnEpisodeAndPriceIsAdded()
        {
            var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
            var episodeDomainModel = EpisodeDomainModel.Get(_fixture.Create<Episode>());
            var episodePriceDomainModel = EpisodePriceDomainModel.Get(_fixture.Create<EpisodePrice>());

            apprenticeship.AddEpisode(
                episodeDomainModel.Ukprn,
                episodeDomainModel.EmployerAccountId,
                episodePriceDomainModel.StartDate,
                episodePriceDomainModel.EndDate,
                episodePriceDomainModel.TotalPrice,
                episodePriceDomainModel.TrainingPrice,
                episodePriceDomainModel.EndPointAssessmentPrice,
                episodeDomainModel.FundingType,
                episodeDomainModel.FundingPlatform,
                episodePriceDomainModel.FundingBandMaximum,
                episodeDomainModel.FundingEmployerAccountId,
                episodeDomainModel.LegalEntityName,
                episodeDomainModel.AccountLegalEntityId,
                episodeDomainModel.TrainingCode,
                episodeDomainModel.TrainingCourseVersion);

            var latestEpisode = apprenticeship.GetEntity().Episodes.Last();
            latestEpisode.Should().BeEquivalentTo(episodeDomainModel);
        }
    }
}
