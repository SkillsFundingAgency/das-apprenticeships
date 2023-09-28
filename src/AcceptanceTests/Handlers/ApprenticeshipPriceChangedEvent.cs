using System.Collections.Concurrent;
using NServiceBus;
using SFA.DAS.Apprenticeships.Types;

namespace SFA.DAS.Apprenticeships.AcceptanceTests.Handlers;

public class ApprenticeshipPriceChangedEventHandler : IHandleMessages<ApprenticeshipPriceChangedEvent>
{
    public static ConcurrentBag<ApprenticeshipPriceChangedEvent> ReceivedEvents { get; } = new();

    public Task Handle(ApprenticeshipPriceChangedEvent message, IMessageHandlerContext context)
    {
        ReceivedEvents.Add(message);
        return Task.CompletedTask;
    }
}