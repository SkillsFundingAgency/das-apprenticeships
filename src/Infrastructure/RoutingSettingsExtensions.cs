using System.Diagnostics.CodeAnalysis;
using NServiceBus;
using SFA.DAS.Apprenticeships.Types;
using SFA.DAS.Approvals.EventHandlers.Messages;

namespace SFA.DAS.Apprenticeships.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public static class RoutingSettingsExtensions
    {
        public static void AddRouting(this RoutingSettings settings)
        {
            settings.RouteToEndpoint(typeof(ApprovalCreatedEvent), QueueNames.ApprovalCreated);
        }
    }
}
