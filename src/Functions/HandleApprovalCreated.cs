using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using SFA.DAS.Apprenticeships.Command;
using SFA.DAS.Apprenticeships.Command.AddApproval;
using SFA.DAS.Apprenticeships.Infrastructure;
using SFA.DAS.Apprenticeships.Types;
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
        public async Task HandleCommand([NServiceBusTrigger(Endpoint = QueueNames.ApprovalCreated)] ApprovalCreatedEvent approvalCreatedEvent)
        {
            await _commandDispatcher.Send(new AddApprovalCommand
            {
                TrainingCode = approvalCreatedEvent.TrainingCode,
                ActualStartDate = approvalCreatedEvent.ActualStartDate,
                AgreedPrice = approvalCreatedEvent.AgreedPrice,
                ApprovalsApprenticeshipId = approvalCreatedEvent.ApprovalsApprenticeshipId,
                EmployerAccountId = approvalCreatedEvent.EmployerAccountId,
                FundingEmployerAccountId = approvalCreatedEvent.FundingEmployerAccountId.GetValueOrDefault(),
                FundingType = approvalCreatedEvent.FundingType.ToFundingType(),
                LegalEntityName = approvalCreatedEvent.LegalEntityName,
                PlannedEndDate = approvalCreatedEvent.PlannedEndDate,
                UKPRN = approvalCreatedEvent.UKPRN,
                Uln = approvalCreatedEvent.Uln
            });
        }
    }
}