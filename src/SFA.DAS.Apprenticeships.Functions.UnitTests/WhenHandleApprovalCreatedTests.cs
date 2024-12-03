using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Command;
using SFA.DAS.Apprenticeships.Command.AddApprenticeship;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.CommitmentsV2.Messages.Events;

namespace SFA.DAS.Apprenticeships.Functions.UnitTests
{
    public class WhenHandleApprovalCreatedTests
    {
        [Test]
        public async Task ThenApprovalIsAdded()
        {
            var fixture = new Fixture();
            var @event = fixture.Create<ApprenticeshipCreatedEvent>();
            var commandDispatcher = new Mock<ICommandDispatcher>();
            var handler = new HandleApprovalCreated(commandDispatcher.Object);
            await handler.HandleCommand(@event, new Mock<ILogger>().Object);

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
                        c.FundingPlatform == (@event.IsOnFlexiPaymentPilot.HasValue ? (@event.IsOnFlexiPaymentPilot.Value ? FundingPlatform.DAS : FundingPlatform.SLD) : null)
                    ),
                    It.IsAny<CancellationToken>()));
        }
    }
}