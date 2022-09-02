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
            var apprenticeship = apprenticeshipFactory.CreateNew("1234435", "TRN");
            apprenticeship.AddApproval(_fixture.Create<long>(), _fixture.Create<long>(), _fixture.Create<long>(), _fixture.Create<string>(), _fixture.Create<DateTime>(), _fixture.Create<DateTime>(), _fixture.Create<decimal>(), _fixture.Create<long>(), _fixture.Create<Enums.FundingType>(), _fixture.Create<int>());
            var approval = apprenticeship.Approvals.Single();
            var command = _fixture.Create<ApprenticeshipCreated>();

            _apprenticeshipRepository.Setup(x => x.Get(command.ApprenticeshipKey)).ReturnsAsync(apprenticeship);

            await _handler.Handle(command);

            _messageSession.Verify(x => x.Publish(It.Is<ApprenticeshipCreatedEvent>(e =>
                    e.ApprenticeshipKey == apprenticeship.Key &&
                    e.Uln == apprenticeship.Uln &&
                    e.TrainingCode == apprenticeship.TrainingCode &&
                    e.FundingEmployerAccountId == approval.FunctionEmployerAccountId &&
                    e.AgreedPrice == approval.AgreedPrice &&
                    e.FundingType == (FundingType)approval.FundingType &&
                    e.ActualStartDate == approval.ActualStartDate &&
                    e.ApprovalsApprenticeshipId == approval.ApprovalsApprenticeshipId &&
                    e.EmployerAccountId == approval.EmployerAccountId &&
                    e.LegalEntityName == approval.LegalEntityName &&
                    e.PlannedEndDate == approval.PlannedEndDate &&
                    e.UKPRN == approval.Ukprn
                ), It.IsAny<PublishOptions>()));
        }
    }
}
