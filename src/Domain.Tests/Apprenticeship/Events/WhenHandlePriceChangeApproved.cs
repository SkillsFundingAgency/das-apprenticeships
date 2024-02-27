using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Events;
using SFA.DAS.Apprenticeships.Domain.Factories;
using SFA.DAS.Apprenticeships.Domain.Repositories;
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
            var apprenticeshipFactory = new ApprenticeshipFactory();
            var apprenticeship = apprenticeshipFactory.CreateNew(
                "1234435",
                "TRN",
                new DateTime(2000,
                    10,
                    16),
                "Ron",
                "Swanson",
                _fixture.Create<decimal?>(),
                _fixture.Create<decimal?>(),
                _fixture.Create<decimal>(),
                _fixture.Create<string>(),
                _fixture.Create<int>(),
                _fixture.Create<DateTime>(),
                _fixture.Create<DateTime>(),
                _fixture.Create<long>(),
                _fixture.Create<long>(),
                _fixture.Create<long>());
            apprenticeship.AddApproval(_fixture.Create<long>(), _fixture.Create<string>(), _fixture.Create<DateTime>(), _fixture.Create<DateTime>(), _fixture.Create<decimal>(), _fixture.Create<long>(), _fixture.Create<Enums.FundingType>(), _fixture.Create<int>(), _fixture.Create<DateTime?>(), _fixture.Create<Enums.FundingPlatform?>());
            apprenticeship.AddPriceHistory(_fixture.Create<decimal>(), _fixture.Create<decimal>(), _fixture.Create<decimal>(), _fixture.Create<DateTime>(), _fixture.Create<DateTime>(), PriceChangeRequestStatus.Created, null, _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<DateTime>(), _fixture.Create<DateTime>());
            apprenticeship.ApprovePriceChange("Bob");
            var approval = apprenticeship.Approvals.Single();
            var priceChange = apprenticeship.PriceHistories.Single();
            var command = new PriceChangeApproved(apprenticeship.Key, priceChange.Key, ApprovedBy.Employer);

            _apprenticeshipRepository.Setup(x => x.Get(command.ApprenticeshipKey)).ReturnsAsync(apprenticeship);

            await _handler.Handle(command);

            _messageSession.Verify(x => x.Publish(It.Is<PriceChangeApprovedEvent>(e =>
                    e.ApprenticeshipKey == apprenticeship.Key &&
                    e.EmployerAccountId == apprenticeship.EmployerAccountId &&
                    e.ApprenticeshipKey == apprenticeship.Key &&
                    e.ApprenticeshipId == approval.ApprovalsApprenticeshipId &&
                    e.EmployerAccountId == apprenticeship.EmployerAccountId &&
                    e.ApprovedDate == priceChange.EmployerApprovedDate!.Value &&
                    e.ApprovedBy == ApprovedBy.Employer &&
                    e.AssessmentPrice == priceChange.AssessmentPrice!.Value &&
                    e.TrainingPrice == priceChange.TrainingPrice!.Value &&
                    e.EffectiveFromDate == priceChange.EffectiveFromDate &&
                    e.ProviderId == apprenticeship.Ukprn
                ), It.IsAny<PublishOptions>()));
        }
    }
}
