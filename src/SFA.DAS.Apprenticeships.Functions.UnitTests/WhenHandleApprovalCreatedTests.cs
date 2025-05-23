using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NServiceBus.Testing;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Command;
using SFA.DAS.Apprenticeships.Command.AddApprenticeship;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.Functions.Handlers;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.Apprenticeships.Functions.UnitTests
{
    public class WhenHandleApprovalCreatedTests
    {
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
        }

        [Test]
        public async Task ThenApprovalIsAdded()
        {
            var @event = _fixture.Create<ApprenticeshipCreatedEvent>();
            var commandDispatcher = new Mock<ICommandDispatcher>();
            var handler = new ApprenticeshipCreatedEventHandler(commandDispatcher.Object, new Mock<ILogger<ApprenticeshipCreatedEventHandler>>().Object);
            await handler.Handle(@event, new TestableMessageHandlerContext());

            commandDispatcher.Verify(x =>
                x.Send(It.Is<AddApprenticeshipCommand>(c =>
                        c.TrainingCode == @event.TrainingCode &&
                        c.ActualStartDate == @event.ActualStartDate &&
                        c.TotalPrice == @event.PriceEpisodes[0].Cost &&
                        c.TrainingPrice == @event.PriceEpisodes[0].TrainingPrice &&
                        c.EndPointAssessmentPrice == @event.PriceEpisodes[0].EndPointAssessmentPrice &&
                        c.ApprovalsApprenticeshipId == @event.ApprenticeshipId &&
                        c.EmployerAccountId == @event.AccountId &&
                        c.FundingEmployerAccountId == @event.TransferSenderId &&
                        c.FundingType == Enums.FundingType.Transfer &&
                        c.LegalEntityName == @event.LegalEntityName &&
                        c.PlannedEndDate == @event.EndDate &&
                        c.UKPRN == @event.ProviderId &&
                        c.Uln == @event.Uln &&
                        c.DateOfBirth == @event.DateOfBirth &&
                        c.FirstName == @event.FirstName &&
                        c.LastName == @event.LastName &&
                        c.ApprenticeshipHashedId == @event.ApprenticeshipHashedId &&
                        c.FundingPlatform == (@event.IsOnFlexiPaymentPilot.HasValue ? (@event.IsOnFlexiPaymentPilot.Value ? FundingPlatform.DAS : FundingPlatform.SLD) : null) &&
                        c.AccountLegalEntityId == @event.AccountLegalEntityId &&
                        c.TrainingCourseVersion == @event.TrainingCourseVersion
                    ),
                    It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task WhenEmployerIsLevyThenFundingTypeIsLevy()
        {
            var @event = _fixture.Build<ApprenticeshipCreatedEvent>().With(x => x.TransferSenderId, (long?)null).With(x => x.ApprenticeshipEmployerTypeOnApproval, ApprenticeshipEmployerType.Levy).Create();
            var commandDispatcher = new Mock<ICommandDispatcher>();
            var handler = new ApprenticeshipCreatedEventHandler(commandDispatcher.Object, new Mock<ILogger<ApprenticeshipCreatedEventHandler>>().Object);
            await handler.Handle(@event, new TestableMessageHandlerContext());

            commandDispatcher.Verify(x =>
                x.Send(It.Is<AddApprenticeshipCommand>(c => c.FundingType == Enums.FundingType.Levy),
                    It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task WhenEmployerIsNonLevyThenFundingTypeIsNonLevy()
        {
            var @event = _fixture.Build<ApprenticeshipCreatedEvent>().With(x => x.TransferSenderId, (long?)null).With(x => x.ApprenticeshipEmployerTypeOnApproval, ApprenticeshipEmployerType.NonLevy).Create();
            var commandDispatcher = new Mock<ICommandDispatcher>();
            var handler = new ApprenticeshipCreatedEventHandler(commandDispatcher.Object, new Mock<ILogger<ApprenticeshipCreatedEventHandler>>().Object);
            await handler.Handle(@event, new TestableMessageHandlerContext());

            commandDispatcher.Verify(x =>
                x.Send(It.Is<AddApprenticeshipCommand>(c => c.FundingType == Enums.FundingType.NonLevy),
                    It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task WhenHasTransferSenderThenFundingTypeIsTransfer()
        {
            var @event = _fixture.Build<ApprenticeshipCreatedEvent>().With(x => x.TransferSenderId, 1234).Create();
            var commandDispatcher = new Mock<ICommandDispatcher>();
            var handler = new ApprenticeshipCreatedEventHandler(commandDispatcher.Object, new Mock<ILogger<ApprenticeshipCreatedEventHandler>>().Object);
            await handler.Handle(@event, new TestableMessageHandlerContext());

            commandDispatcher.Verify(x =>
                x.Send(It.Is<AddApprenticeshipCommand>(c => c.FundingType == Enums.FundingType.Transfer),
                    It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task WhenNoActualStartDateThenPlannedStartDateUsed()
        {
            var @event = _fixture.Build<ApprenticeshipCreatedEvent>().Without(x => x.ActualStartDate).Create();
            var commandDispatcher = new Mock<ICommandDispatcher>();
            var handler = new ApprenticeshipCreatedEventHandler(commandDispatcher.Object, new Mock<ILogger<ApprenticeshipCreatedEventHandler>>().Object);
            await handler.Handle(@event, new TestableMessageHandlerContext());

            commandDispatcher.Verify(x =>
                x.Send(It.Is<AddApprenticeshipCommand>(c => c.ActualStartDate == @event.StartDate),
                    It.IsAny<CancellationToken>()));
        }
    }
}