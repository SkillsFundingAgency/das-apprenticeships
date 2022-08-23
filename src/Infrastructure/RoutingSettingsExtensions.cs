using System.Diagnostics.CodeAnalysis;
using NServiceBus;
using SFA.DAS.Apprenticeships.Types;

namespace SFA.DAS.Apprenticeships.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public static class RoutingSettingsExtensions
    {
        public static void AddRouting(this RoutingSettings settings)
        {
            settings.RouteToEndpoint(typeof(ApprovalCreatedCommand), QueueNames.ApprovalCreated);
        }
    }
}
