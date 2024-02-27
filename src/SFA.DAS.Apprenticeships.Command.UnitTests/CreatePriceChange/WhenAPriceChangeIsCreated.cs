using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Command.CreatePriceChange;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;

namespace SFA.DAS.Apprenticeships.Command.UnitTests.CreatePriceChange
{
    [TestFixture]
    public class WhenAPriceChangeIsCreated
    {
        private CreatePriceChangeCommandHandler _commandHandler = null!;
        private Mock<IApprenticeshipRepository> _apprenticeshipRepository = null!;
        private Fixture _fixture = null!;

        [SetUp]
        public void SetUp()
        {
            _apprenticeshipRepository = new Mock<IApprenticeshipRepository>();
            _commandHandler = new CreatePriceChangeCommandHandler(_apprenticeshipRepository.Object);

            _fixture = new Fixture();
            _fixture.Customize(new ApprenticeshipCustomization());
        }

        [Test]
        public async Task ThenPriceHistoryIsAddedToApprenticeship()
        {
            var command = _fixture.Create<CreatePriceChangeCommand>();
            command.Requester = PriceChangeRequester.Provider.ToString();
            var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();

            _apprenticeshipRepository.Setup(x => x.Get(command.ApprenticeshipKey)).ReturnsAsync(apprenticeship);
            
            await _commandHandler.Handle(command);
            
            _apprenticeshipRepository.Verify(x => x.Update(It.Is<ApprenticeshipDomainModel>(y => y.GetEntity().PriceHistories.Count == 1)));
        }

        [TestCase("Provider")]
        [TestCase("Employer")]
        public async Task ThenCorrectPriceHistoryValuesAreSet(string requester)
        {
            var command = _fixture.Create<CreatePriceChangeCommand>();
            command.Requester = requester;
            
            var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();

            _apprenticeshipRepository.Setup(x => x.Get(command.ApprenticeshipKey)).ReturnsAsync(apprenticeship);
            
            await _commandHandler.Handle(command);

            if (requester == PriceChangeRequester.Provider.ToString())
                _apprenticeshipRepository.Verify(x => x.Update(It.Is<ApprenticeshipDomainModel>(y =>
                    y.GetEntity().PriceHistories.Single().TrainingPrice == command.TrainingPrice &&
                    y.GetEntity().PriceHistories.Single().AssessmentPrice == command.AssessmentPrice &&
                    y.GetEntity().PriceHistories.Single().TotalPrice == command.TotalPrice &&
                    y.GetEntity().PriceHistories.Single().EffectiveFromDate == command.EffectiveFromDate &&
                    y.GetEntity().PriceHistories.Single().CreatedDate != DateTime.MinValue &&
                    y.GetEntity().PriceHistories.Single().PriceChangeRequestStatus == PriceChangeRequestStatus.Created &&
                    y.GetEntity().PriceHistories.Single().ProviderApprovedBy == command.UserId &&
                    y.GetEntity().PriceHistories.Single().EmployerApprovedBy == null
                )));
            else
                _apprenticeshipRepository.Verify(x => x.Update(It.Is<ApprenticeshipDomainModel>(y =>
                    y.GetEntity().PriceHistories.Single().TrainingPrice == command.TrainingPrice &&
                    y.GetEntity().PriceHistories.Single().AssessmentPrice == command.AssessmentPrice &&
                    y.GetEntity().PriceHistories.Single().TotalPrice == command.TotalPrice &&
                    y.GetEntity().PriceHistories.Single().EffectiveFromDate == command.EffectiveFromDate &&
                    y.GetEntity().PriceHistories.Single().CreatedDate != DateTime.MinValue &&
                    y.GetEntity().PriceHistories.Single().PriceChangeRequestStatus == PriceChangeRequestStatus.Created &&
                    y.GetEntity().PriceHistories.Single().ProviderApprovedBy == null &&
                    y.GetEntity().PriceHistories.Single().EmployerApprovedBy == command.UserId
                )));
        }

        [Test]
        public void ThenAnExceptionIsThrownIfTheRequesterIsNotSet()
        {
            var command = _fixture.Create<CreatePriceChangeCommand>();
            command.Requester = string.Empty;
                
            var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();

            _apprenticeshipRepository.Setup(x => x.Get(command.ApprenticeshipKey)).ReturnsAsync(apprenticeship);

            Assert.ThrowsAsync<ArgumentException>(() => _commandHandler.Handle(command));
        }
    }
}
