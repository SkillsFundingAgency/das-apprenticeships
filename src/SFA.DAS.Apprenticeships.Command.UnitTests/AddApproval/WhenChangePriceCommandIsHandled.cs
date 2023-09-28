using AutoFixture;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Command.ChangePrice;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Infrastructure.Services;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;
using SFA.DAS.Apprenticeships.Types;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Apprenticeships.Command.UnitTests.AddApproval;

[TestFixture]
public class WhenChangePriceCommandIsHandled
{
    private ChangePriceCommandHandler _commandHandler;
    private Mock<IApprenticeshipRepository> _apprenticeshipRepository;
    private Mock<IFundingBandMaximumService> _fundingBandMaximumService;
    private Fixture _fixture;
    private Mock<IMessageSession> _messageSession;

    [SetUp]
    public void SetUp()
    {
        _apprenticeshipRepository = new Mock<IApprenticeshipRepository>();
        _fundingBandMaximumService = new Mock<IFundingBandMaximumService>();
        _messageSession = new Mock<IMessageSession>();
        _commandHandler = new ChangePriceCommandHandler(_apprenticeshipRepository.Object, _fundingBandMaximumService.Object, _messageSession.Object);

        _fixture = new Fixture();
        _fixture.Customize(new ApprenticeshipCustomization());
    }

    [Test]
    public async Task ThenPriceChangeHistoryRecordIsAddedToTheDatabase()
    {
        var command = _fixture.Create<ChangePriceCommand>();
        var apprenticeship = _fixture.Create<Apprenticeship>();

        _apprenticeshipRepository.Setup(r => r.GetByApprenticeshipId(command.ApprovalsApprenticeshipId))
            .ReturnsAsync(apprenticeship);

        await _commandHandler.Handle(command);

        _apprenticeshipRepository.Verify(x => x.Update(apprenticeship), Times.Once);

        _apprenticeshipRepository.Verify(x => x.Update(It.Is<Apprenticeship>(
                y =>
                    y.PriceHistory.Count == 1 &&
                    y.PriceHistory.First().AssessmentPrice == command.AssessmentPrice &&
                    y.PriceHistory.First().ApprovedDate == command.ApprovedDate &&
                    y.PriceHistory.First().EffectiveFrom == command.EffectiveFrom &&
                    y.PriceHistory.First().TrainingPrice == command.TrainingPrice
            ))
            , Times.Once);
    }

    [Test]
    public async Task ThenApprenticeshipPriceChangedEventIsPublished()
    {
        var command = _fixture.Create<ChangePriceCommand>();
        var apprenticeship = _fixture.Create<Apprenticeship>();

        _apprenticeshipRepository.Setup(r => r.GetByApprenticeshipId(command.ApprovalsApprenticeshipId))
            .ReturnsAsync(apprenticeship);

        await _commandHandler.Handle(command);

        _apprenticeshipRepository.Verify(x => x.Update(apprenticeship), Times.Once);

        _messageSession.Verify(x => x.Publish(It.Is<ApprenticeshipPriceChangedEvent>(
                y =>
                    y.ApprenticeshipKey == apprenticeship.Key &&
                    y.AssessmentPrice == command.AssessmentPrice &&
                    y.ApprovedDate == command.ApprovedDate &&
                    y.EffectiveFrom == command.EffectiveFrom &&
                    y.TrainingPrice == command.TrainingPrice &&
                    y.TotalPrice == command.AssessmentPrice + command.TrainingPrice
            ), It.IsAny<PublishOptions>())
            , Times.Once);
    }
}