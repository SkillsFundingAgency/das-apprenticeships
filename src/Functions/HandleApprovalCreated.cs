using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using SFA.DAS.Apprenticeships.Command;
using SFA.DAS.Apprenticeships.Command.AddApproval;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Approvals.EventHandlers.Messages;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;

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
        public async Task HandleCommand([NServiceBusTrigger(Endpoint = QueueNames.ApprovalCreated)] ApprovalCreatedEvent @event)
        {
            await _commandDispatcher.Send(new AddApprovalCommand
            {
                TrainingCode = @event.TrainingCode,
                ActualStartDate = @event.ActualStartDate,
                AgreedPrice = @event.AgreedPrice,
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
                PlannedStartDate = @event.StartDate,
                ApprenticeshipHashedId = @event.ApprenticeshipHashedId,
                FundingPlatform = @event.IsOnFlexiPaymentPilot.HasValue ? (@event.IsOnFlexiPaymentPilot.Value ? FundingPlatform.DAS : FundingPlatform.SLD) : null,
                AccountLegalEntityId = @event.AccountLegalEntityId,
                TrainingCourseVersion = @event.TrainingCourseVersion
            });
        }

    }
}