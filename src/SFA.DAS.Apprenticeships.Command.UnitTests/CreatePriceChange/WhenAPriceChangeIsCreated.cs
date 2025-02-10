using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Command.CreatePriceChange;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.Infrastructure.Services;
using SFA.DAS.Apprenticeships.TestHelpers;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;
using SFA.DAS.Apprenticeships.Types;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Apprenticeships.Command.UnitTests.CreatePriceChange;

[TestFixture]
public class WhenAPriceChangeIsCreated
{
    private CreatePriceChangeCommandHandler _commandHandler = null!;
    private Mock<IApprenticeshipRepository> _apprenticeshipRepository = null!;
    private Mock<IMessageSession> _messageSession = null!;
    private Mock<ISystemClockService> _systemClockService = null!;
    private DateTime _createdDate = DateTime.UtcNow;
    private Mock<ILogger<CreatePriceChangeCommandHandler>> _logger = null!;
    private Fixture _fixture = null!;

    [SetUp]
    public void SetUp()
    {
        _apprenticeshipRepository = new Mock<IApprenticeshipRepository>();
        _messageSession = new Mock<IMessageSession>();
        _systemClockService = new Mock<ISystemClockService>();
        _systemClockService.Setup(x => x.UtcNow).Returns(_createdDate);
        _logger = new Mock<ILogger<CreatePriceChangeCommandHandler>>();
        _commandHandler = new CreatePriceChangeCommandHandler(
            _apprenticeshipRepository.Object, _messageSession.Object, _systemClockService.Object, _logger.Object);

        _fixture = new Fixture();
        _fixture.Customize(new ApprenticeshipCustomization());
    }

    [Test]
    public async Task ThenPriceHistoryIsAddedToApprenticeship()
    {
        var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
        ApprenticeshipDomainModelTestHelper.AddEpisode(apprenticeship);
        var command = _fixture.Create<CreatePriceChangeCommand>();
        command.Initiator = ChangeInitiator.Provider.ToString();
        command.EffectiveFromDate = apprenticeship.LatestPrice.StartDate.AddDays(_fixture.Create<int>());
        _apprenticeshipRepository.Setup(x => x.Get(command.ApprenticeshipKey)).ReturnsAsync(apprenticeship);
        
        await _commandHandler.Handle(command);
        
        _apprenticeshipRepository.Verify(x => x.Update(It.Is<ApprenticeshipDomainModel>(y => y.GetEntity().PriceHistories.Count == 1)));
    }

    [TestCase("Provider")]
    [TestCase("Employer")]
    public async Task ThenCorrectPriceHistoryValuesAreSet(string initiator)
    {
        var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
        ApprenticeshipDomainModelTestHelper.AddEpisode(apprenticeship);
        var command = _fixture.Create<CreatePriceChangeCommand>();
        command.Initiator = initiator;
        command.AssessmentPrice = apprenticeship.LatestPrice.GetEntity().EndPointAssessmentPrice + 1;
        command.TrainingPrice = apprenticeship.LatestPrice.GetEntity().TrainingPrice + 1;
        command.TotalPrice = (decimal)(command.TrainingPrice! + command.AssessmentPrice!);
        command.EffectiveFromDate = apprenticeship.LatestPrice.StartDate.AddDays(_fixture.Create<int>());

        apprenticeship.LatestPrice.GetEntity().TotalPrice = command.TotalPrice - 1;

        _apprenticeshipRepository.Setup(x => x.Get(command.ApprenticeshipKey)).ReturnsAsync(apprenticeship);

        await _commandHandler.Handle(command);

        if (initiator == ChangeInitiator.Provider.ToString())
            _apprenticeshipRepository.Verify(x => x.Update(It.Is<ApprenticeshipDomainModel>(y =>
                y.GetEntity().PriceHistories.Single().TrainingPrice == command.TrainingPrice &&
                y.GetEntity().PriceHistories.Single().AssessmentPrice == command.AssessmentPrice &&
                y.GetEntity().PriceHistories.Single().TotalPrice == command.TotalPrice &&
                y.GetEntity().PriceHistories.Single().EffectiveFromDate == command.EffectiveFromDate &&
                y.GetEntity().PriceHistories.Single().CreatedDate != DateTime.MinValue &&
                y.GetEntity().PriceHistories.Single().PriceChangeRequestStatus == ChangeRequestStatus.Created &&
                y.GetEntity().PriceHistories.Single().ProviderApprovedBy == command.UserId &&
                y.GetEntity().PriceHistories.Single().EmployerApprovedBy == null &&
                y.GetEntity().PriceHistories.Single().Initiator == ChangeInitiator.Provider
            )));
        else
            _apprenticeshipRepository.Verify(x => x.Update(It.Is<ApprenticeshipDomainModel>(y =>
                y.GetEntity().PriceHistories.Single().TrainingPrice == command.TrainingPrice &&
                y.GetEntity().PriceHistories.Single().AssessmentPrice == command.AssessmentPrice &&
                y.GetEntity().PriceHistories.Single().TotalPrice == command.TotalPrice &&
                y.GetEntity().PriceHistories.Single().EffectiveFromDate == command.EffectiveFromDate &&
                y.GetEntity().PriceHistories.Single().CreatedDate != DateTime.MinValue &&
                y.GetEntity().PriceHistories.Single().PriceChangeRequestStatus == ChangeRequestStatus.Created &&
                y.GetEntity().PriceHistories.Single().ProviderApprovedBy == null &&
                y.GetEntity().PriceHistories.Single().EmployerApprovedBy == command.UserId &&
                y.GetEntity().PriceHistories.Single().Initiator == ChangeInitiator.Employer
            )));
    }

		[TestCase(5000, 5001, false)]
		[TestCase(5000, 5000, true)]
		[TestCase(5000, 4999, true)]
		public async Task ThenPriceChangeIsAutoApprovedCorrectly(decimal oldTotal, decimal newTotal, bool expectAutoApprove)
		{
			var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
        ApprenticeshipDomainModelTestHelper.AddEpisode(apprenticeship);
			var command = _fixture.Create<CreatePriceChangeCommand>();
			command.Initiator = ChangeInitiator.Provider.ToString();
			command.AssessmentPrice = newTotal*0.25m;
			command.TrainingPrice = newTotal - command.AssessmentPrice;
			command.TotalPrice = (decimal)(command.TrainingPrice! + command.AssessmentPrice!);
        command.EffectiveFromDate = apprenticeship.LatestPrice.StartDate.AddDays(_fixture.Create<int>());

			apprenticeship.LatestPrice.GetEntity().TotalPrice = oldTotal;

			_apprenticeshipRepository.Setup(x => x.Get(command.ApprenticeshipKey)).ReturnsAsync(apprenticeship);

			await _commandHandler.Handle(command);

        if (expectAutoApprove)
        {
            _apprenticeshipRepository.Verify(x => x.Update(It.Is<ApprenticeshipDomainModel>(y =>
                y.GetEntity().PriceHistories.Single().PriceChangeRequestStatus == ChangeRequestStatus.Approved)));

            AssertEventPublished(apprenticeship, command.EffectiveFromDate);
        }
        else
        {
            _apprenticeshipRepository.Verify(x => x.Update(It.Is<ApprenticeshipDomainModel>(y =>
                y.GetEntity().PriceHistories.Single().PriceChangeRequestStatus == ChangeRequestStatus.Approved)), Times.Never);
        }
    }

		[Test]
    public void ThenAnExceptionIsThrownIfTheRequesterIsNotSet()
    {
        var command = _fixture.Create<CreatePriceChangeCommand>();
        command.Initiator = string.Empty;
            
        var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();

        _apprenticeshipRepository.Setup(x => x.Get(command.ApprenticeshipKey)).ReturnsAsync(apprenticeship);

        Assert.ThrowsAsync<ArgumentException>(() => _commandHandler.Handle(command));
    }

    private void AssertEventPublished(ApprenticeshipDomainModel apprenticeship, DateTime effectiveFromDate)
    {
        _messageSession.Verify(x => x.Publish(It.Is<ApprenticeshipPriceChangedEvent>(e =>
            DoApprenticeshipDetailsMatchDomainModel(e, apprenticeship) &&
            e.ApprovedDate == _createdDate &&
            e.ApprovedBy == ApprovedBy.Provider &&
            e.EffectiveFromDate == effectiveFromDate &&
            ApprenticeshipDomainModelTestHelper.DoEpisodeDetailsMatchDomainModel(e, apprenticeship)), It.IsAny<PublishOptions>()));
    }

    private static bool DoApprenticeshipDetailsMatchDomainModel(ApprenticeshipPriceChangedEvent e, ApprenticeshipDomainModel apprenticeship)
    {
        return
            e.ApprenticeshipKey == apprenticeship.Key &&
            e.ApprenticeshipId == apprenticeship.ApprovalsApprenticeshipId;
    }
}
