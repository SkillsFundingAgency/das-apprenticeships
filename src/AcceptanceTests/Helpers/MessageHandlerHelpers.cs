namespace SFA.DAS.Learning.AcceptanceTests.Helpers
{
    internal static class MessageHandlerHelper
    {
        internal static IEnumerable<MessageHandler> GetMessageHandlers()
        {
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => x.GetName().FullName.Contains("SFA.DAS"));

            var result = allAssemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.GetInterfaces()
                    .Any(interfaceType =>
                        interfaceType.IsGenericType &&
                        interfaceType.GetGenericTypeDefinition() == typeof(IHandleMessages<>)))
                .SelectMany(matchingClass => matchingClass.GetInterfaces(),
                    (matchingClass, handlerInterface) => new { matchingClass, handlerInterface })
                .Where(t => t.handlerInterface.IsGenericType &&
                            t.handlerInterface.GetGenericTypeDefinition() == typeof(IHandleMessages<>))
                .Select(t => new MessageHandler
                {
                    HandlerType = t.matchingClass,
                    HandledEventType = t.handlerInterface.GetGenericArguments()[0]
                }).ToList();

            return result;
        }
    }

    internal class MessageHandler
    {
        public Type HandlerType { get; set; }
        public Type HandledEventType { get; set; }
    }
}
