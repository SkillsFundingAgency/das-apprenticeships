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
            var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();

            _apprenticeshipRepository.Setup(x => x.Get(command.ApprenticeshipKey)).ReturnsAsync(apprenticeship);
            
            await _commandHandler.Handle(command);
            
            _apprenticeshipRepository.Verify(x => x.Add(It.Is<ApprenticeshipDomainModel>(y => y.GetEntity().PriceHistories.Count == 1)));
        }

        [Test]
        public async Task ThenCorrectPriceHistoryValuesAreSet()
        {
            var command = _fixture.Create<CreateApprenticeshipPriceChangeRequest>();
            var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();

            _apprenticeshipRepository.Setup(x => x.Get(command.ApprenticeshipKey)).ReturnsAsync(apprenticeship);
            
            await _commandHandler.Handle(command);
            
            _apprenticeshipRepository.Verify(x => x.Add(It.Is<ApprenticeshipDomainModel>(y => 
                y.GetEntity().PriceHistories.Single().TrainingPrice == command.TrainingPrice &&
                y.GetEntity().PriceHistories.Single().AssessmentPrice == command.AssessmentPrice &&
                y.GetEntity().PriceHistories.Single().TotalPrice == command.TotalPrice &&
                y.GetEntity().PriceHistories.Single().EffectiveFromDate == command.EffectiveFromDate &&
                y.GetEntity().PriceHistories.Single().CreatedDate != DateTime.MinValue &&
                y.GetEntity().PriceHistories.Single().PriceChangeRequestStatus == PriceChangeRequestStatus.Created.ToString()
            )));
        }
    }
}
