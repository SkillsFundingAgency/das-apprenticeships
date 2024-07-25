using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Events;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Domain.UnitTests.Helpers;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;
using SFA.DAS.Apprenticeships.Types;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Apprenticeship.Events
{
    public class WhenHandlePriceChangeApproved
    {
        private Mock<IApprenticeshipRepository> _apprenticeshipRepository;
        private Mock<IMessageSession> _messageSession;
        private PriceChangeApprovedHandler _handler;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _fixture.Customize(new ApprenticeshipCustomization());
            _apprenticeshipRepository = new Mock<IApprenticeshipRepository>();
            _messageSession = new Mock<IMessageSession>();
            _handler = new PriceChangeApprovedHandler(_apprenticeshipRepository.Object, _messageSession.Object);
        }

        [Test]
        public async Task ByEmployerThenPriceChangeApprovedEventIsPublished()
        {
            var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
            ApprenticeshipDomainModelTestHelper.AddEpisode(apprenticeship);
            ApprenticeshipDomainModelTestHelper.AddPendingPriceChangeProviderInitiated(apprenticeship, effectiveFromDate:apprenticeship.LatestPrice.StartDate.AddDays(_fixture.Create<int>()));
            apprenticeship.ApprovePriceChange("Bob", null, null);
            var priceChange = apprenticeship.PriceHistories.First(x => x.PriceChangeRequestStatus == ChangeRequestStatus.Approved);
            var domainEvent = new PriceChangeApproved(apprenticeship.Key, priceChange.Key, ApprovedBy.Employer, _fixture.Create<EpisodeDomainModel.AmendedPrices>());

            _apprenticeshipRepository.Setup(x => x.Get(domainEvent.ApprenticeshipKey)).ReturnsAsync(apprenticeship);

            await _handler.Handle(domainEvent);

            _messageSession.Verify(x => x.Publish(It.Is<OldApprenticeshipPriceChangedEvent>(e =>
                    e.ApprenticeshipKey == apprenticeship.Key &&
                    e.EmployerAccountId == apprenticeship.LatestEpisode.EmployerAccountId &&
                    e.ApprenticeshipKey == apprenticeship.Key &&
                    e.ApprenticeshipId == apprenticeship.ApprovalsApprenticeshipId &&
                    e.EmployerAccountId == apprenticeship.LatestEpisode.EmployerAccountId &&
                    e.ApprovedDate == priceChange.EmployerApprovedDate!.Value &&
                    e.ApprovedBy == ApprovedBy.Employer &&
                    e.AssessmentPrice == priceChange.AssessmentPrice!.Value &&
                    e.TrainingPrice == priceChange.TrainingPrice!.Value &&
                    e.EffectiveFromDate == priceChange.EffectiveFromDate &&
                    e.ProviderId == apprenticeship.LatestEpisode.Ukprn &&
                    e.EpisodeKey == domainEvent.AmendedPrices.ApprenticeshipEpisodeKey &&
                    e.PriceKey == domainEvent.AmendedPrices.LatestEpisodePrice.GetEntity().Key &&
                    e.DeletedPriceKeys == domainEvent.AmendedPrices.DeletedPriceKeys
                ), It.IsAny<PublishOptions>()));
        }

        [Test]
        public async Task ByProviderThenPriceChangeApprovedEventIsPublished()
        {
            var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
            ApprenticeshipDomainModelTestHelper.AddEpisode(apprenticeship);
            var trainingPrice = _fixture.Create<int>();
            var assessmentPrice = _fixture.Create<int>();
            ApprenticeshipDomainModelTestHelper.AddPendingPriceChangeEmployerInitiated(
                apprenticeship, 
                totalPrice: trainingPrice + assessmentPrice,
                effectiveFromDate:apprenticeship.LatestPrice.StartDate.AddDays(_fixture.Create<int>()));
            apprenticeship.ApprovePriceChange("Bob", trainingPrice, assessmentPrice);
            var priceChange = apprenticeship.PriceHistories.First(x => x.PriceChangeRequestStatus == ChangeRequestStatus.Approved);
            var domainEvent = new PriceChangeApproved(apprenticeship.Key, priceChange.Key, ApprovedBy.Provider, _fixture.Create<EpisodeDomainModel.AmendedPrices>());

            _apprenticeshipRepository.Setup(x => x.Get(domainEvent.ApprenticeshipKey)).ReturnsAsync(apprenticeship);

            await _handler.Handle(domainEvent);

            _messageSession.Verify(x => x.Publish(It.Is<OldApprenticeshipPriceChangedEvent>(e =>
                    e.ApprenticeshipKey == apprenticeship.Key &&
                    e.EmployerAccountId == apprenticeship.LatestEpisode.EmployerAccountId &&
                    e.ApprenticeshipKey == apprenticeship.Key &&
                    e.ApprenticeshipId == apprenticeship.ApprovalsApprenticeshipId &&
                    e.EmployerAccountId == apprenticeship.LatestEpisode.EmployerAccountId &&
                    e.ApprovedDate == priceChange.ProviderApprovedDate!.Value &&
                    e.ApprovedBy == ApprovedBy.Provider &&
                    e.AssessmentPrice == priceChange.AssessmentPrice!.Value &&
                    e.TrainingPrice == priceChange.TrainingPrice!.Value &&
                    e.EffectiveFromDate == priceChange.EffectiveFromDate &&
                    e.ProviderId == apprenticeship.LatestEpisode.Ukprn &&
                    e.EpisodeKey == domainEvent.AmendedPrices.ApprenticeshipEpisodeKey &&
                    e.PriceKey == domainEvent.AmendedPrices.LatestEpisodePrice.GetEntity().Key &&
                    e.DeletedPriceKeys == domainEvent.AmendedPrices.DeletedPriceKeys
                ), It.IsAny<PublishOptions>()));
        }
    }
}
