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
        public async Task HandleCommand([NServiceBusTrigger(Endpoint = QueueNames.ApprovalCreated)] ApprovalCreatedEvent command)
        {
            await _commandDispatcher.Send(new AddApprovalCommand
            {
                TrainingCode = command.TrainingCode,
                ActualStartDate = command.ActualStartDate,
                AgreedPrice = command.AgreedPrice,
                ApprovalsApprenticeshipId = command.ApprovalsApprenticeshipId,
                EmployerAccountId = command.EmployerAccountId,
                FundingEmployerAccountId = command.FundingEmployerAccountId.GetValueOrDefault(),
                FundingType = command.FundingType.ToFundingType(),
                LegalEntityName = command.LegalEntityName,
                PlannedEndDate = command.PlannedEndDate,
                UKPRN = command.UKPRN.GetValueOrDefault(),
                Uln = command.Uln
            });
        }
    }
}