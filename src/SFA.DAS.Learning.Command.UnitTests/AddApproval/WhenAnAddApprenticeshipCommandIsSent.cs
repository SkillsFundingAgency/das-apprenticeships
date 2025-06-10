using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Learning.Command.AddLearning;
using SFA.DAS.Learning.Domain.Apprenticeship;
using SFA.DAS.Learning.Domain.Factories;
using SFA.DAS.Learning.Domain.Repositories;
using SFA.DAS.Learning.Infrastructure.Services;
using SFA.DAS.Learning.TestHelpers;
using SFA.DAS.Learning.TestHelpers.AutoFixture.Customizations;
using SFA.DAS.Learning.Types;
using FundingPlatform = SFA.DAS.Learning.Enums.FundingPlatform;

namespace SFA.DAS.Learning.Command.UnitTests.AddApproval;

[TestFixture]
public class WhenAnAddApprenticeshipCommandIsSent
{
    private AddLearningCommandHandler _commandHandler = null!;
    private Mock<ILearningFactory> _apprenticeshipFactory = null!;
    private Mock<ILearningRepository> _apprenticeshipRepository = null!;
    private Mock<IFundingBandMaximumService> _fundingBandMaximumService = null!;
    private Mock<IMessageSession> _messageSession = null!;
    private Mock<ILogger<AddLearningCommandHandler>> _logger = null!;
    private Fixture _fixture = null!;

    [SetUp]
    public void SetUp()
    {
        _apprenticeshipFactory = new Mock<ILearningFactory>();
        _apprenticeshipRepository = new Mock<ILearningRepository>();
        _fundingBandMaximumService = new Mock<IFundingBandMaximumService>();
        _messageSession = new Mock<IMessageSession>();
        _logger = new Mock<ILogger<AddLearningCommandHandler>>();
        _commandHandler = new AddLearningCommandHandler(
            _apprenticeshipFactory.Object, 
            _apprenticeshipRepository.Object, 
            _fundingBandMaximumService.Object,
            _messageSession.Object,
            _logger.Object);

        _fixture = new Fixture();
        _fixture.Customize(new ApprenticeshipCustomization());
    }

    [Test]
	public async Task WhenAnApprenticeshipAlreadyExistsThenItIsNotCreatedAgain()
    {
        var command = _fixture.Create<AddLearningCommand>();
        var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();

		_apprenticeshipRepository.Setup(x => x.Get(command.Uln, command.ApprovalsApprenticeshipId)).ReturnsAsync(apprenticeship);

        await _commandHandler.Handle(command);

        _apprenticeshipRepository.Verify(x => x.Add(It.IsAny<ApprenticeshipDomainModel>()), Times.Never());
    }
	
    [Test]
    public async Task ThenAnEpisodeIsCreated()
    {
        var command = _fixture.Create<AddLearningCommand>();
        var trainingCodeInt = _fixture.Create<int>();
        command.TrainingCode = trainingCodeInt.ToString();
        var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();

        _apprenticeshipFactory.Setup(x => x.CreateNew(
                command.ApprovalsApprenticeshipId,
                command.Uln,
                command.DateOfBirth,
                command.FirstName,
                command.LastName,
                command.ApprenticeshipHashedId))
            .Returns(apprenticeship);
        
        _fundingBandMaximumService
            .Setup(x => x.GetFundingBandMaximum(trainingCodeInt, It.IsAny<DateTime?>()))
            .ReturnsAsync((int)Math.Ceiling(command.TotalPrice));

        await _commandHandler.Handle(command);

        _apprenticeshipRepository.Verify(x => x.Add(It.Is<ApprenticeshipDomainModel>(y => y.GetEntity().Episodes.Count == 1)));
        _apprenticeshipRepository.Verify(x => x.Add(It.Is<ApprenticeshipDomainModel>(y => y.GetEntity().Episodes.Single().Prices.Count == 1)));
    }

    [Test]
    public async Task ThenAnEpisodePriceIsCreatedWithTheCorrectFundingBandMaximum()
    {
        var command = _fixture.Create<AddLearningCommand>();
        var trainingCodeInt = _fixture.Create<int>();
        command.TrainingCode = trainingCodeInt.ToString();
        var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
        var fundingBandMaximum = _fixture.Create<int>();

        _apprenticeshipFactory.Setup(x => x.CreateNew(
                command.ApprovalsApprenticeshipId,
                command.Uln,
                command.DateOfBirth,
                command.FirstName,
                command.LastName,
                command.ApprenticeshipHashedId))
            .Returns(apprenticeship);

        _fundingBandMaximumService
            .Setup(x => x.GetFundingBandMaximum(trainingCodeInt, It.IsAny<DateTime?>()))
            .ReturnsAsync(fundingBandMaximum);

        await _commandHandler.Handle(command);

        _apprenticeshipRepository
            .Verify(x => x.Add(It.Is<ApprenticeshipDomainModel>(y => 
                y.GetEntity().Episodes.Last().Prices.Last().FundingBandMaximum == fundingBandMaximum)));
    }

    [Test]
    public async Task ThenExceptionIsThrownWhenNoFundingBandMaximumForTheGivenDateAndCourseCodeIsPresent()
    {
        var command = _fixture.Create<AddLearningCommand>();
        var trainingCodeInt = _fixture.Create<int>();
        command.TrainingCode = trainingCodeInt.ToString();

        _fundingBandMaximumService.Setup(x => x.GetFundingBandMaximum(trainingCodeInt, It.IsAny<DateTime?>()))
            .ReturnsAsync((int?)null);

        await _commandHandler.Invoking(x => x.Handle(command, It.IsAny<CancellationToken>())).Should()
            .ThrowAsync<Exception>()
            .WithMessage(
                $"No funding band maximum found for course {command.TrainingCode} for given date {command.ActualStartDate?.ToString("u")}. Approvals Apprenticeship Id: {command.ApprovalsApprenticeshipId}");
    }

    [Test]
    public async Task ThenFundingBandMaximumIsFetchedUsingStartDate()
    {
        var command = _fixture.Create<AddLearningCommand>();
        var trainingCodeInt = _fixture.Create<int>();
        command.TrainingCode = trainingCodeInt.ToString();
        var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
        var fundingBandMaximum = _fixture.Create<int>();

        _fundingBandMaximumService.Setup(x => x.GetFundingBandMaximum(trainingCodeInt, It.IsAny<DateTime?>()))
            .ReturnsAsync(fundingBandMaximum);

        _apprenticeshipFactory.Setup(x => x.CreateNew(
                command.ApprovalsApprenticeshipId,
                command.Uln,
                command.DateOfBirth,
                command.FirstName,
                command.LastName,
                command.ApprenticeshipHashedId))
            .Returns(apprenticeship);

        await _commandHandler.Handle(command);

        _fundingBandMaximumService.Verify(x => x.GetFundingBandMaximum(trainingCodeInt, command.ActualStartDate));
    }

    [Test]
    public async Task ThenEventPublished()
    {
        // Arrange
        var command = _fixture.Create<AddLearningCommand>();
        command.FundingPlatform = FundingPlatform.DAS;
        var trainingCodeInt = _fixture.Create<int>();
        command.TrainingCode = trainingCodeInt.ToString();
        var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();

        _apprenticeshipFactory.Setup(x => x.CreateNew(
                command.ApprovalsApprenticeshipId,
                command.Uln,
                command.DateOfBirth,
                command.FirstName,
                command.LastName,
                command.ApprenticeshipHashedId))
            .Returns(apprenticeship);

        _fundingBandMaximumService
            .Setup(x => x.GetFundingBandMaximum(trainingCodeInt, It.IsAny<DateTime?>()))
            .ReturnsAsync((int)Math.Ceiling(command.TotalPrice));

        // Act
        await _commandHandler.Handle(command);

        // Assert
        _messageSession.Verify(x => x.Publish(It.Is<ApprenticeshipCreatedEvent>(e =>
            DoApprenticeshipDetailsMatchDomainModel(e, apprenticeship)
            && ApprenticeshipDomainModelTestHelper.DoEpisodeDetailsMatchDomainModel(e, apprenticeship)), It.IsAny<PublishOptions>(),
            It.IsAny<CancellationToken>()));
    }

    [Test]
    public async Task AndNotFundedByDASThenEventIsNotPublished()
    {
        // Arrange
        var command = _fixture.Create<AddLearningCommand>();
        command.FundingPlatform = FundingPlatform.SLD;
        var trainingCodeInt = _fixture.Create<int>();
        command.TrainingCode = trainingCodeInt.ToString();
        var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();

        _apprenticeshipFactory.Setup(x => x.CreateNew(
                command.ApprovalsApprenticeshipId,
                command.Uln,
                command.DateOfBirth,
                command.FirstName,
                command.LastName,
                command.ApprenticeshipHashedId))
            .Returns(apprenticeship);

        _fundingBandMaximumService
            .Setup(x => x.GetFundingBandMaximum(trainingCodeInt, It.IsAny<DateTime?>()))
            .ReturnsAsync((int)Math.Ceiling(command.TotalPrice));

        // Act
        await _commandHandler.Handle(command);

        // Assert
        _messageSession.Verify(x => x.Publish(It.IsAny<ApprenticeshipCreatedEvent>(), It.IsAny<PublishOptions>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static bool DoApprenticeshipDetailsMatchDomainModel(ApprenticeshipCreatedEvent e, ApprenticeshipDomainModel apprenticeship)
    {
        return
            e.ApprenticeshipKey == apprenticeship.Key &&
            e.ApprovalsApprenticeshipId == apprenticeship.ApprovalsApprenticeshipId &&
            e.Uln == apprenticeship.Uln &&
            e.FirstName == apprenticeship.FirstName &&
            e.LastName == apprenticeship.LastName &&
            e.DateOfBirth == apprenticeship.DateOfBirth;
    }
}