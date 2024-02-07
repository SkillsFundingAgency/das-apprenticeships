using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Command.AddPriceHistory;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;

namespace SFA.DAS.Apprenticeships.Command.UnitTests.AppPriceHistory
{
    [TestFixture]
    public class WhenAPriceHistoryIsAdded
    {
        private CreateApprenticeshipPriceChangeCommandHandler _commandHandler = null!;
        private Mock<IApprenticeshipRepository> _apprenticeshipRepository = null!;
        private Fixture _fixture = null!;

        private const string Provider = "Provider";
        private const string Employer = "Employer";

        [SetUp]
        public void SetUp()
        {
            _apprenticeshipRepository = new Mock<IApprenticeshipRepository>();
            _commandHandler = new CreateApprenticeshipPriceChangeCommandHandler(_apprenticeshipRepository.Object);

            _fixture = new Fixture();
            _fixture.Customize(new ApprenticeshipCustomization());
        }

        [Test]
        public async Task ThenPriceHistoryIsAddedToApprenticeship()
        {
            var command = _fixture.Create<CreateApprenticeshipPriceChangeRequest>();
            command.EmployerId = null;
            var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();

            _apprenticeshipRepository.Setup(x => x.Get(command.ApprenticeshipKey)).ReturnsAsync(apprenticeship);
            
            await _commandHandler.Handle(command);
            
            _apprenticeshipRepository.Verify(x => x.Update(It.Is<ApprenticeshipDomainModel>(y => y.GetEntity().PriceHistories.Count == 1)));
        }

        [TestCase(Provider)]
        [TestCase(Employer)]
        public async Task ThenCorrectPriceHistoryValuesAreSet(string initiator)
        {
            var command = _fixture.Create<CreateApprenticeshipPriceChangeRequest>();
            if (initiator == Provider)
                command.EmployerId = null;
            else
                command.ProviderId = null;
            
            var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();

            _apprenticeshipRepository.Setup(x => x.Get(command.ApprenticeshipKey)).ReturnsAsync(apprenticeship);
            
            await _commandHandler.Handle(command);

            if (initiator == Provider)
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

        [TestCase(true)]
        [TestCase(false)]
        public void ThenAnExceptionIsThrownIfTheRequestIsNotInitiatedByASingleUserType(bool valuesSet)
        {
            var command = _fixture.Create<CreateApprenticeshipPriceChangeRequest>();
            if (!valuesSet)
            {
                command.EmployerId = null;
                command.ProviderId = null;
            }
                
            var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();

            _apprenticeshipRepository.Setup(x => x.Get(command.ApprenticeshipKey)).ReturnsAsync(apprenticeship);

            Assert.ThrowsAsync<ArgumentException>(() => _commandHandler.Handle(command));
        }
    }
}
