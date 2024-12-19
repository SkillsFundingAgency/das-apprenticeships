using AutoFixture;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Events;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;
using SFA.DAS.Apprenticeships.Types;
using System.Threading.Tasks;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Apprenticeship.Events;

[TestFixture]
public class WhenHandleWithdrawn
{
    private Mock<IMessageSession> _messageSession;
    private WithdrawnEventHandler _handler;
    private Fixture _fixture;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _fixture.Customize(new ApprenticeshipCustomization());
        _messageSession = new Mock<IMessageSession>();
        _handler = new WithdrawnEventHandler(_messageSession.Object);
    }

    [Test]
    public async Task ByEmployerThenApprenticeshipStartDateChangedEventIsPublished()
    {
        // Arrange

        var command = _fixture.Create<WithdrawnEvent>();

        // Act
        await _handler.Handle(command);

        // Assert
        _messageSession.Verify(x => x.Publish(It.Is<ApprenticeshipWithdrawnEvent>(e =>
            e.ApprenticeshipKey == command.ApprenticeshipKey &&
            e.ApprenticeshipId == command.ApprenticeshipId &&
            e.Reason == command.Reason &&
            e.LastDayOfLearning == command.LastDayOfLearning
            ), It.IsAny<PublishOptions>()));
    }

}