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
            _apprenticeshipRepository = new Mock<IApprenticeshipRepository>();
            _messageSession = new Mock<IMessageSession>();
            _handler = new ApprenticeshipCreatedHandler(_apprenticeshipRepository.Object, _messageSession.Object);
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
                _fixture.Create<DateTime>());
            apprenticeship.AddApproval(_fixture.Create<long>(), _fixture.Create<long>(), _fixture.Create<long>(), _fixture.Create<string>(), _fixture.Create<DateTime>(), _fixture.Create<DateTime>(), _fixture.Create<decimal>(), _fixture.Create<long>(), _fixture.Create<Enums.FundingType>(), _fixture.Create<int>(), _fixture.Create<DateTime?>(), _fixture.Create<Enums.FundingPlatform?>());
            var approval = apprenticeship.Approvals.Single();
            var command = _fixture.Create<ApprenticeshipCreated>();

            _apprenticeshipRepository.Setup(x => x.Get(command.ApprenticeshipKey)).ReturnsAsync(apprenticeship);

            await _handler.Handle(command);

            _messageSession.Verify(x => x.Publish(It.Is<ApprenticeshipCreatedEvent>(e =>
                    e.ApprenticeshipKey == apprenticeship.Key &&
                    e.Uln == apprenticeship.Uln &&
                    e.TrainingCode == apprenticeship.TrainingCode &&
                    e.FundingEmployerAccountId == approval.FundingEmployerAccountId &&
                    e.AgreedPrice == approval.AgreedPrice &&
                    e.FundingType == (FundingType)approval.FundingType &&
                    e.ActualStartDate == approval.ActualStartDate &&
                    e.ApprovalsApprenticeshipId == approval.ApprovalsApprenticeshipId &&
                    e.EmployerAccountId == approval.EmployerAccountId &&
                    e.LegalEntityName == approval.LegalEntityName &&
                    e.PlannedEndDate == approval.PlannedEndDate &&
                    e.UKPRN == approval.Ukprn &&
                    e.DateOfBirth == apprenticeship.DateOfBirth &&
                    e.FirstName == apprenticeship.FirstName &&
                    e.LastName== apprenticeship.LastName &&
                    e.AgeAtStartOfApprenticeship == apprenticeship.AgeAtStartOfApprenticeship &&
                    e.PlannedStartDate == approval.PlannedStartDate &&
                    e.FundingPlatform == (FundingPlatform?)approval.FundingPlatform
                ), It.IsAny<PublishOptions>()));
        }
    }
}
