using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Apprenticeships.Command;
using SFA.DAS.Apprenticeships.Command.AddApprenticeship;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Approvals.EventHandlers.Messages;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace SFA.DAS.Apprenticeships.Functions
{
    public class HandleApprovalCreated
    {
        private readonly ICommandDispatcher _commandDispatcher;

        public HandleApprovalCreated(ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
        }

        [FunctionName("HandleApprovalCreatedEvent")]
        public async Task HandleCommand([NServiceBusTrigger(Endpoint = QueueNames.ApprovalCreated)] ApprovalCreatedEvent @event,
            ILogger log)
        {
            if (!@event.IsOnFlexiPaymentPilot.GetValueOrDefault())
            {
                log.LogInformation("Apprenticeship {hashedId} is not funded by DAS and therefore, no further action will be taken.", @event.ApprenticeshipHashedId);
                return;
            }
            await _commandDispatcher.Send(new AddApprenticeshipCommand
            {
                TrainingCode = @event.TrainingCode,
                ActualStartDate = @event.ActualStartDate,
                TotalPrice = @event.AgreedPrice,
                TrainingPrice = @event.TrainingPrice,
                EndPointAssessmentPrice = @event.EndPointAssessmentPrice,
                ApprovalsApprenticeshipId = @event.ApprovalsApprenticeshipId,
                EmployerAccountId = @event.EmployerAccountId,
                FundingEmployerAccountId = @event.FundingEmployerAccountId,
                FundingType = Enum.Parse<Enums.FundingType>(@event.FundingType.ToString()),
                LegalEntityName = @event.LegalEntityName,
                PlannedEndDate = @event.PlannedEndDate.Value,
                UKPRN = @event.UKPRN,
                Uln = @event.Uln,
                DateOfBirth = @event.DateOfBirth,
                FirstName = @event.FirstName,
                LastName = @event.LastName,
                ApprenticeshipHashedId = @event.ApprenticeshipHashedId,
                FundingPlatform = @event.IsOnFlexiPaymentPilot.HasValue ? (@event.IsOnFlexiPaymentPilot.Value ? FundingPlatform.DAS : FundingPlatform.SLD) : null,
                AccountLegalEntityId = @event.AccountLegalEntityId,
                TrainingCourseVersion = @event.TrainingCourseVersion
            });
        }
    }
}