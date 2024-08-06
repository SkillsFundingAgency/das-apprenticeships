using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Command;
using SFA.DAS.Apprenticeships.Command.AddApprenticeship;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Approvals.EventHandlers.Messages;
using FundingType = SFA.DAS.Approvals.EventHandlers.Messages.FundingType;

namespace SFA.DAS.Apprenticeships.Functions.UnitTests
{
    public class WhenHandleApprovalCreatedTests
    {
        [Test]
        public async Task ThenApprovalIsAdded()
        {
            var fixture = new Fixture();
            var @event = fixture.Create<ApprovalCreatedEvent>();
            @event.FundingType = FundingType.Transfer;
            var commandDispatcher = new Mock<ICommandDispatcher>();
            var handler = new HandleApprovalCreated(commandDispatcher.Object);
            await handler.HandleCommand(@event, new Mock<ILogger>().Object);

            commandDispatcher.Verify(x =>
                x.Send(It.Is<AddApprenticeshipCommand>(c =>
                        c.TrainingCode == @event.TrainingCode &&
                        c.ActualStartDate == @event.ActualStartDate &&
                        c.TotalPrice == @event.AgreedPrice &&
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
                        c.FundingPlatform == (@event.IsOnFlexiPaymentPilot.HasValue ? (@event.IsOnFlexiPaymentPilot.Value ? FundingPlatform.DAS : FundingPlatform.SLD) : null)
                    ),
                    It.IsAny<CancellationToken>()));
        }
    }
}