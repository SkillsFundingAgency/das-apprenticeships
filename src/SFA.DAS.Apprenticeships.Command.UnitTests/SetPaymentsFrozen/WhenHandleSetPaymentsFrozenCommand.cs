using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Command.SetPaymentsFrozen;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.TestHelpers;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;
using SFA.DAS.Apprenticeships.Types;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Apprenticeships.Command.UnitTests.SetPaymentsFrozen;

public class WhenHandleSetPaymentsFrozenCommand
{
    private Mock<IApprenticeshipRepository> _mockApprenticeshipRepository;
    private Mock<IMessageSession> _messageSession;
    private Mock<ILogger<SetPaymentsFrozenCommandHandler>> _logger;
    private SetPaymentsFrozenCommandHandler _handler;
    private Fixture _fixture;

    [SetUp]
    public void SetUp()
    {
        _mockApprenticeshipRepository = new Mock<IApprenticeshipRepository>();
        _messageSession = new Mock<IMessageSession>();
        _logger = new Mock<ILogger<SetPaymentsFrozenCommandHandler>>();
        _handler = new SetPaymentsFrozenCommandHandler(
            _mockApprenticeshipRepository.Object, _messageSession.Object, _logger.Object);
        _fixture = new Fixture();
        _fixture.Customize(new ApprenticeshipCustomization());
    }

    [TestCase(SetPayments.Freeze)]
    [TestCase(SetPayments.Unfreeze)]
    public async Task Handle_ShouldGetAndUpdateApprenticeship(SetPayments setPayments)
    {
        // Arrange
        var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
        ApprenticeshipDomainModelTestHelper.AddEpisode(apprenticeship);
        _mockApprenticeshipRepository.Setup(x => x.Get(It.IsAny<Guid>())).ReturnsAsync(apprenticeship);

        if(setPayments == SetPayments.Unfreeze)
            apprenticeship.SetPaymentsFrozen(true, _fixture.Create<string>(), DateTime.Now, _fixture.Create<string>());

        var command = new SetPaymentsFrozenCommand(
            _fixture.Create<Guid>(), _fixture.Create<string>(), setPayments, _fixture.Create<string>());

        // Act
        await _handler.Handle(command);

        // Assert
        _mockApprenticeshipRepository.Verify(x => x.Get(command.ApprenticeshipKey), Times.Once);
        _mockApprenticeshipRepository.Verify(x => x.Update(
            It.Is<ApprenticeshipDomainModel>(y => y
                .LatestEpisode.PaymentsFrozen == command.NewPaymentsFrozenStatus)), Times.Once);

        switch(setPayments)
        {
            case SetPayments.Freeze:
                _messageSession.Verify(x => x.Publish(
                    It.Is<PaymentsFrozenEvent>(e => e.ApprenticeshipKey == command.ApprenticeshipKey),
                    It.IsAny<PublishOptions>(),
                    It.IsAny<CancellationToken>()),
                    Times.Once);
                break;

            case SetPayments.Unfreeze:
                _messageSession.Verify(x => x.Publish(
                    It.Is<PaymentsUnfrozenEvent>(e => e.ApprenticeshipKey == command.ApprenticeshipKey),
                    It.IsAny<PublishOptions>(),
                    It.IsAny<CancellationToken>()),
                    Times.Once);
                break;
        }
    }
}
