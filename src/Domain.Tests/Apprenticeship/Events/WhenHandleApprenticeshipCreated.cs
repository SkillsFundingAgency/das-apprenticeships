﻿using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Events;
using SFA.DAS.Apprenticeships.Domain.Factories;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Types;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Apprenticeship.Events
{
    public class WhenHandleApprenticeshipCreated
    {
        private Mock<IApprenticeshipRepository> _apprenticeshipRepository;
        private Mock<IEventPublisher> _eventPublisher;
        private ApprenticeshipCreatedHandler _handler;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _apprenticeshipRepository = new Mock<IApprenticeshipRepository>();
            _eventPublisher = new Mock<IEventPublisher>();
            _handler = new ApprenticeshipCreatedHandler(_apprenticeshipRepository.Object, _eventPublisher.Object);
        }

        [Test]
        public async Task ThenApprenticeshipCreatedEventIsPublished()
        {
            var apprenticeshipFactory = new ApprenticeshipFactory();
            var apprenticeship = apprenticeshipFactory.CreateNew("1234435", "TRN", new DateTime(2000, 10, 16));
            apprenticeship.AddApproval(_fixture.Create<long>(), _fixture.Create<long>(), _fixture.Create<long>(), _fixture.Create<string>(), _fixture.Create<DateTime>(), _fixture.Create<DateTime>(), _fixture.Create<decimal>(), _fixture.Create<long>(), _fixture.Create<Enums.FundingType>());
            var approval = apprenticeship.Approvals.Single();
            var command = _fixture.Create<ApprenticeshipCreated>();

            _apprenticeshipRepository.Setup(x => x.Get(command.ApprenticeshipKey)).ReturnsAsync(apprenticeship);

            await _handler.Handle(command);

            _eventPublisher.Verify(x => x.Publish<ApprenticeshipCreatedEvent>(It.Is<ApprenticeshipCreatedEvent>(e =>
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
                    e.UKPRN == approval.Ukprn &&
                    e.DateOfBirth == apprenticeship.DateOfBirth &&
                    e.AgeAtStartOfApprenticeship == apprenticeship.AgeAtStartOfApprenticeship
                )));
        }
    }
}