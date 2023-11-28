using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Command.AddPriceHistory;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Factories;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;

namespace SFA.DAS.Apprenticeships.Command.UnitTests.AddApproval
{
    [TestFixture]
    public class WhenAPriceHistoryIsAdded
    {
        private CreateApprenticeshipPriceChangeCommandHandler _commandHandler = null!;
        private Mock<IApprenticeshipRepository> _apprenticeshipRepository = null!;
        private Mock<IApprenticeshipFactory> _apprenticeshipFactory = null!;
        private Fixture _fixture = null!;

        [SetUp]
        public void SetUp()
        {
            _apprenticeshipRepository = new Mock<IApprenticeshipRepository>();
            _apprenticeshipFactory = new Mock<IApprenticeshipFactory>();
            _commandHandler = new CreateApprenticeshipPriceChangeCommandHandler(_apprenticeshipRepository.Object, _apprenticeshipFactory.Object);

            _fixture = new Fixture();
            _fixture.Customize(new ApprenticeshipCustomization());
        }

        [Test]
        public async Task ThenPriceHistoryIsAddedToApprenticeship()
        {
            var command = _fixture.Create<CreateApprenticeshipPriceChangeRequest>();
            var apprenticeship = _fixture.Create<Apprenticeship>();

            _apprenticeshipRepository.Setup(x => x.Get(command.ApprenticeshipKey)).ReturnsAsync(apprenticeship);
            
            await _commandHandler.Handle(command);
            
            _apprenticeshipRepository.Verify(x => x.Add(It.Is<Apprenticeship>(y => y.GetModel().PriceHistories.Count == 1)));
        }

        [Test]
        public async Task ThenCorrectPriceHistoryValuesAreSet()
        {
            var command = _fixture.Create<CreateApprenticeshipPriceChangeRequest>();
            var apprenticeship = _fixture.Create<Apprenticeship>();

            _apprenticeshipRepository.Setup(x => x.Get(command.ApprenticeshipKey)).ReturnsAsync(apprenticeship);
            
            await _commandHandler.Handle(command);
            
            _apprenticeshipRepository.Verify(x => x.Add(It.Is<Apprenticeship>(y => 
                y.GetModel().PriceHistories.Single().TrainingPrice == command.TrainingPrice &&
                y.GetModel().PriceHistories.Single().AssessmentPrice == command.AssessmentPrice &&
                y.GetModel().PriceHistories.Single().TotalPrice == command.TotalPrice &&
                y.GetModel().PriceHistories.Single().EffectiveFromDate == command.EffectiveFromDate &&
                y.GetModel().PriceHistories.Single().CreatedDate != DateTime.MinValue &&
                y.GetModel().PriceHistories.Single().PriceChangeRequestStatus == PriceChangeRequestStatus.Created
            )));
        }
    }
}
