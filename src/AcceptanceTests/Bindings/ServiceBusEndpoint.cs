using SFA.DAS.Apprenticeships.AcceptanceTests.Helpers;
using SFA.DAS.Apprenticeships.Functions;
using SFA.DAS.Apprenticeships.Types;
using SFA.DAS.Approvals.EventHandlers.Messages;

namespace SFA.DAS.Apprenticeships.AcceptanceTests.Bindings
{
    [Binding]
    public static class ServiceBusEndpoint
    {
        [BeforeScenario]
        public static async Task StartEndpoint(TestContext context)
        {
            context.EndpointInstance = await EndpointHelper
                .StartEndpoint(QueueNames.ApprovalCreated + ".test", false, new[]
                {
                    typeof(ApprovalCreatedEvent), 
                    typeof(ApprenticeshipCreatedEvent),
                    typeof(ApprenticeshipPriceChangedEvent)
                });
        }

        [AfterScenario]
        public static async Task StopEndpoint(TestContext context)
        {
            await context.EndpointInstance.Stop().ConfigureAwait(false);
        }
    }
}
