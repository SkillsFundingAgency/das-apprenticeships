using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Command.SetPaymentsFrozen;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Apprenticeships.Command.UnitTests.SetPaymentsFrozen;

public class WhenHandleSetPaymentsFrozenCommand
{
    private Mock<IApprenticeshipRepository> _mockApprenticeshipRepository;
    private SetPaymentsFrozenCommandHandler _handler;
    private Fixture _fixture;

    [SetUp]
    public void SetUp()
    {
        _mockApprenticeshipRepository = new Mock<IApprenticeshipRepository>();
        _handler = new SetPaymentsFrozenCommandHandler(_mockApprenticeshipRepository.Object);
        _fixture = new Fixture();
        _fixture.Customize(new ApprenticeshipCustomization());
    }

    [Test]
    public async Task Handle_ShouldGetAndUpdateApprenticeship()
    {
        // Arrange
        var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
        ApprenticeshipDomainModelTestHelper.AddEpisode(apprenticeship);
        _mockApprenticeshipRepository.Setup(x => x.Get(It.IsAny<Guid>())).ReturnsAsync(apprenticeship);
        var command = _fixture.Create<SetPaymentsFrozenCommand>();

        // Act
        await _handler.Handle(command);

        // Assert
        _mockApprenticeshipRepository.Verify(x => x.Get(command.ApprenticeshipKey), Times.Once);
        _mockApprenticeshipRepository.Verify(x => x.Update(
            It.Is<ApprenticeshipDomainModel>(y => y
                .LatestEpisode.PaymentsFrozen == command.NewPaymentsFrozenStatus)), Times.Once);
    }
}
