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
            _apprenticeshipRepository = new Mock<IApprenticeshipRepository>();
            _messageSession = new Mock<IMessageSession>();
            _handler = new PriceChangeApprovedHandler(_apprenticeshipRepository.Object, _messageSession.Object);
        }

        [Test]
        public async Task ThenApprenticeshipCreatedEventIsPublished()
        {
            var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
            ApprenticeshipDomainModelTestHelper.AddEpisode(apprenticeship);
            ApprenticeshipDomainModelTestHelper.AddPendingPriceChange(apprenticeship);
            apprenticeship.ApprovePriceChange("Bob", null, null);
            var priceChange = apprenticeship.PriceHistories.Single();
            var command = new PriceChangeApproved(apprenticeship.Key, priceChange.Key, ApprovedBy.Employer);

            _apprenticeshipRepository.Setup(x => x.Get(command.ApprenticeshipKey)).ReturnsAsync(apprenticeship);

            await _handler.Handle(command);

            _messageSession.Verify(x => x.Publish(It.Is<PriceChangeApprovedEvent>(e =>
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
                    e.ProviderId == apprenticeship.LatestEpisode.Ukprn
                ), It.IsAny<PublishOptions>()));
        }
    }
}
