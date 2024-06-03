using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Events;
using SFA.DAS.Apprenticeships.Types;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Apprenticeship.Events;

public class WhenHandlePaymentsFrozen
{
    private Mock<IMessageSession> _messageSessionMock;

    [SetUp]
    public void SetUp()
    {
        _messageSessionMock = new Mock<IMessageSession>(); 
    }

    [Test]
    public async Task Handle_ShouldPublishPaymentsFrozenEvent()
    {
        // Arrange
        var handler = new PaymentsFrozenHandler(_messageSessionMock.Object);
        var @event = new PaymentsFrozen(Guid.NewGuid());

        // Act
        await handler.Handle(@event);

        // Assert
        _messageSessionMock.Verify(x => x.Publish(
            It.Is<PaymentsFrozenEvent>(e => e.ApprenticeshipKey == @event.ApprenticeshipKey), 
            It.IsAny<PublishOptions>()), 
            Times.Once);
    }
}