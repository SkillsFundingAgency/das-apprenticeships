using Microsoft.Azure.Functions.Worker;
using System.Reflection;

namespace SFA.DAS.Apprenticeships.AcceptanceTests.Helpers;

internal static class QueueFunctionResolver
{
    internal static IEnumerable<QueueTriggeredFunction> GetQueueTriggeredFunctions()
    {
        var allAssemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(x => x.GetName().FullName.Contains("SFA.DAS"));

        var matchingClasses = allAssemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type
                .GetMethods()
                .Any(method => method.GetParameters()
                .Any(parameter => parameter.GetCustomAttributes(typeof(ServiceBusTriggerAttribute), false)
                .Any())));

        var queueTriggeredFunctions = new List<QueueTriggeredFunction>();
        foreach (var matchingClass in matchingClasses)
        {
            var endpoints = matchingClass.GetMethods()
                .SelectMany(method => method.GetParameters()
                .Where(parameter => parameter.GetCustomAttributes(typeof(ServiceBusTriggerAttribute), false)
                .Any())
                .Select(parameter => new QueueTriggerEndpoint
                {
                    EventType = parameter.ParameterType,
                    MethodInfo = method
                }));

            queueTriggeredFunctions.Add(new QueueTriggeredFunction
            {
                ClassType = matchingClass,
                Endpoints = endpoints
            });
        }
        return queueTriggeredFunctions;
    }
}

internal class QueueTriggeredFunction
{
    public Type ClassType { get; set; }
    public IEnumerable<QueueTriggerEndpoint> Endpoints { get; set; }
}

internal class QueueTriggerEndpoint
{
    public Type EventType { get; set; }
    public MethodInfo MethodInfo { get; set; }
}
