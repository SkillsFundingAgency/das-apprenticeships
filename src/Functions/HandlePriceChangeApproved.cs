using Microsoft.Azure.WebJobs;
using NServiceBus;
using SFA.DAS.Apprenticeships.Command;
using SFA.DAS.Apprenticeships.Command.ChangePrice;
using SFA.DAS.Approvals.EventHandlers.Messages;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Apprenticeships.Functions
{
    public class HandlePriceChangeApproved
    {
        private readonly ICommandDispatcher _commandDispatcher;

        public HandlePriceChangeApproved(ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
        }

        [FunctionName("HandlePriceChangeApprovedEvent")]
        public async Task HandleCommand([NServiceBusTrigger(Endpoint = "TODO")] PriceChangeApprovedByEmployer @event)
        {
            await _commandDispatcher.Send(new ChangePriceCommand
            {
                ApprovalsApprenticeshipId = @event.ApprenticeshipId,
                ApprovedDate = @event.ApprovedDate,
                AssessmentPrice = @event.AssessmentPrice,
                EffectiveFrom = @event.EffectiveFrom,
                EmployerAccountId = @event.EmployerAccountId,
                TrainingPrice = @event.TrainingPrice,
            });
        }

    }
}

namespace SFA.DAS.Approvals.EventHandlers.Messages // TODO: Move to Approvals
{
    public class PriceChangeApprovedByEmployer : IEvent
    {
        public long ApprenticeshipId { get; set; }
        public decimal TrainingPrice { get; set; }
        public decimal AssessmentPrice { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime ApprovedDate { get; set; }
        public long EmployerAccountId { get; set; }
    }
}