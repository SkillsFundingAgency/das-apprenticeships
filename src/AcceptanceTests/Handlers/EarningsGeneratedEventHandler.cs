using System.Collections.Concurrent;
using NServiceBus;
using SFA.DAS.Apprenticeships.Types;

namespace SFA.DAS.Apprenticeships.AcceptanceTests.Handlers;

public class ApprenticeshipCreatedEventHandler : IHandleMessages<OldApprenticeshipCreatedEvent>
{
    public static ConcurrentBag<OldApprenticeshipCreatedEvent> ReceivedEvents { get; } = new();

    public Task Handle(OldApprenticeshipCreatedEvent message, IMessageHandlerContext context)
    {
        ReceivedEvents.Add(message);
        return Task.CompletedTask;
    }
}