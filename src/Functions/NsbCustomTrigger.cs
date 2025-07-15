using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using NServiceBus;

namespace SFA.DAS.Learning.Functions
{
    /// <summary>
    /// This replaces the NServiceBusTriggerFunction attribute, which is nerfed by any HttpTriggers in the same project
    /// </summary>
    /// <param name="functionEndpoint"></param>
    public class NsbCustomTrigger(IFunctionEndpoint functionEndpoint)
    {
        /// <summary>
        /// This replaces the NServiceBusTriggerFunction attribute, which is nerfed by any HttpTriggers in the same project.
        /// "OldNsbCustomTrigger" is a temporary function to handle events during transition to the new endpoint name
        /// This causes all events to be handled twice, but the handler is idempotent and we are protected by a unique constraint.
        /// This endpoint will be removed in the next deployment.
        /// </summary>
        /// <param name="functionEndpoint"></param>
        public class OldNsbCustomTrigger(IFunctionEndpoint functionEndpoint)
        {
            [Function("OldNServiceBusTriggerFunction")]
            public async Task Run(
                [ServiceBusTrigger(Constants.OldEndpointName, Connection = "AzureWebJobsServiceBus")]
                ServiceBusReceivedMessage message, ServiceBusMessageActions messageActions, FunctionContext context,
                CancellationToken cancellationToken = default)
            {
                await functionEndpoint.Process(message, messageActions, context, cancellationToken);
            }
        }

        [Function("NServiceBusTriggerFunction")]
        public async Task Run(
            [ServiceBusTrigger(Constants.EndpointName, Connection = "AzureWebJobsServiceBus")]
            ServiceBusReceivedMessage message, ServiceBusMessageActions messageActions, FunctionContext context,
            CancellationToken cancellationToken = default)
        {
            await functionEndpoint.Process(message, messageActions, context, cancellationToken);
        }
    }
}
