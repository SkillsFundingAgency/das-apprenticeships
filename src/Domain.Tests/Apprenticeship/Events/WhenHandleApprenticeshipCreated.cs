using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Events;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Domain.UnitTests.Helpers;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;
using SFA.DAS.Apprenticeships.Types;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Apprenticeship.Events
{
    public class WhenHandleApprenticeshipCreated
    {
        private Mock<IApprenticeshipRepository> _apprenticeshipRepository;
        private Mock<IMessageSession> _messageSession;
        private ApprenticeshipCreatedHandler _handler;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _fixture.Customize(new ApprenticeshipCustomization());
            _apprenticeshipRepository = new Mock<IApprenticeshipRepository>();
            _messageSession = new Mock<IMessageSession>();
            _handler = new ApprenticeshipCreatedHandler(_apprenticeshipRepository.Object, _messageSession.Object);
        }

        [Test]
        public async Task ThenApprenticeshipCreatedEventIsPublished()
        {
            var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
            ApprenticeshipDomainModelTestHelper.AddEpisode(apprenticeship);
            var episode = apprenticeship.LatestEpisode;
            var episodePrice = apprenticeship.LatestPrice;
            var command = _fixture.Create<ApprenticeshipCreated>();

            _apprenticeshipRepository.Setup(x => x.Get(command.ApprenticeshipKey)).ReturnsAsync(apprenticeship);

            await _handler.Handle(command);

            _messageSession.Verify(x => x.Publish(It.Is<ApprenticeshipCreatedEvent>(e =>
                    e.ApprenticeshipKey == apprenticeship.Key &&
                    e.Uln == apprenticeship.Uln &&
                    e.TrainingCode == episode.TrainingCode &&
                    e.FundingEmployerAccountId == episode.FundingEmployerAccountId &&
                    e.AgreedPrice == episodePrice.TotalPrice &&
                    e.FundingType == (FundingType)episode.FundingType &&
                    e.ActualStartDate == episodePrice.StartDate &&
                    e.ApprovalsApprenticeshipId == apprenticeship.ApprovalsApprenticeshipId &&
                    e.EmployerAccountId == episode.EmployerAccountId &&
                    e.LegalEntityName == episode.LegalEntityName &&
                    e.PlannedEndDate == episodePrice.EndDate &&
                    e.UKPRN == episode.Ukprn &&
                    e.DateOfBirth == apprenticeship.DateOfBirth &&
                    e.FirstName == apprenticeship.FirstName &&
                    e.LastName == apprenticeship.LastName &&
                    e.AgeAtStartOfApprenticeship == apprenticeship.AgeAtStartOfApprenticeship &&
                    e.FundingPlatform == (FundingPlatform?)episode.FundingPlatform
                ), It.IsAny<PublishOptions>()));
        }
    }
}
