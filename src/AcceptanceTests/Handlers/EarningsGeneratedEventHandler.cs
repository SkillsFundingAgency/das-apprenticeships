using System.Collections.Concurrent;
using NServiceBus;
using SFA.DAS.Apprenticeships.Types;

namespace SFA.DAS.Apprenticeships.AcceptanceTests.Handlers;

public class ApprenticeshipCreatedEventHandler : IHandleMessages<ApprenticeshipCreatedEvent>
{
    public static ConcurrentBag<ApprenticeshipCreatedEvent> ReceivedEvents { get; } = new();

    public Task Handle(ApprenticeshipCreatedEvent message, IMessageHandlerContext context)
    {
        ReceivedEvents.Add(message);
        return Task.CompletedTask;
    }
}