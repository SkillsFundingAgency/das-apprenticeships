using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Command;
using SFA.DAS.Apprenticeships.Command.AddApproval;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Approvals.EventHandlers.Messages;
using FundingType = SFA.DAS.Approvals.EventHandlers.Messages.FundingType;

namespace SFA.DAS.Apprenticeships.Functions.UnitTests
{
    public class WhenHandleApprovalCreatedTests
    {
        private Mock<ICommandDispatcher> _commandDispatcher;
        private HandleApprovalCreated _handler;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _commandDispatcher = new Mock<ICommandDispatcher>();
            _handler = new HandleApprovalCreated(_commandDispatcher.Object);
        }

        [Test]
        public async Task ThenApprovalIsAdded()
        {
            var @event = _fixture.Create<ApprovalCreatedEvent>();
            @event.FundingType = FundingType.Transfer;
            await _handler.HandleCommand(@event);

            _commandDispatcher.Verify(x =>
                x.Send<AddApprovalCommand>(It.Is<AddApprovalCommand>(c =>
                        c.TrainingCode == @event.TrainingCode &&
                        c.ActualStartDate == @event.ActualStartDate &&
                        c.AgreedPrice == @event.AgreedPrice &&
                        c.TrainingPrice == @event.TrainingPrice &&
                        c.EndPointAssessmentPrice == @event.EndPointAssessmentPrice &&
                        c.ApprovalsApprenticeshipId == @event.ApprovalsApprenticeshipId &&
                        c.EmployerAccountId == @event.EmployerAccountId &&
                        c.FundingEmployerAccountId == @event.FundingEmployerAccountId &&
                        c.FundingType == Enums.FundingType.Transfer &&
                        c.LegalEntityName == @event.LegalEntityName &&
                        c.PlannedEndDate == @event.PlannedEndDate &&
                        c.UKPRN == @event.UKPRN &&
                        c.Uln == @event.Uln &&
                        c.DateOfBirth == @event.DateOfBirth &&
                        c.PlannedStartDate == @event.StartDate &&
                        c.FundingPlatform == (@event.IsOnFlexiPaymentPilot.HasValue ? (@event.IsOnFlexiPaymentPilot.Value ? FundingPlatform.DAS : FundingPlatform.SLD) : null)
                    ),
                    It.IsAny<CancellationToken>()));
        }
    }
}