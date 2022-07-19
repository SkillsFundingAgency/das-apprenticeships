using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using SFA.DAS.Apprenticeships.Command;
using SFA.DAS.Apprenticeships.Command.AddApproval;
using SFA.DAS.Apprenticeships.Infrastructure;
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

        [FunctionName("HandleApprovalCreatedCommand")]
        public async Task HandleCommand([NServiceBusTrigger(Endpoint = QueueNames.ApprovalCreated)] ApprovalCreatedCommand command)
        {
            await _commandDispatcher.Send(new AddApprovalCommand());
        }
    }
}