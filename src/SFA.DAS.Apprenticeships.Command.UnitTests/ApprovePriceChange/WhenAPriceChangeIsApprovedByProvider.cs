using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Command.ApprovePriceChange;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;

namespace SFA.DAS.Apprenticeships.Command.UnitTests.ApprovePriceChange
{
    [TestFixture]
    public class WhenAPriceChangeIsApprovedByProvider
    {
        private ApprovePriceChangeCommandHandler _commandHandler = null!;
        private Mock<IApprenticeshipRepository> _apprenticeshipRepository = null!;
        private Fixture _fixture = null!;

        [SetUp]
        public void SetUp()
        {
            _apprenticeshipRepository = new Mock<IApprenticeshipRepository>();
            _commandHandler = new ApprovePriceChangeCommandHandler(_apprenticeshipRepository.Object);

            _fixture = new Fixture();
            _fixture.Customize(new ApprenticeshipCustomization());
        }

        [Test]
        public async Task ThenThePriceHistoryIsApproved()
        {
            //Arrange
            var command = _fixture.Create<ApprovePriceChangeCommand>();
            var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
            var totalPrice = command.TrainingPrice!.Value + command.AssessmentPrice!.Value;
            apprenticeship.AddPriceHistory(null, null, totalPrice, _fixture.Create<DateTime>(), _fixture.Create<DateTime>(), ChangeRequestStatus.Created, _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<DateTime>(), _fixture.Create<DateTime>(), ChangeInitiator.Employer);
            _apprenticeshipRepository.Setup(x => x.Get(command.ApprenticeshipKey)).ReturnsAsync(apprenticeship);

            //Act
            await _commandHandler.Handle(command);
            
            //Assert
            _apprenticeshipRepository.Verify(x => x.Update(
                It.Is<ApprenticeshipDomainModel>(y => y
                        .GetEntity()
                        .PriceHistories
                        .Count(z => z.PriceChangeRequestStatus == ChangeRequestStatus.Approved 
                                    && z.ProviderApprovedBy == command.UserId
                                    && z.TrainingPrice == command.TrainingPrice
                                    && z.AssessmentPrice == command.AssessmentPrice) == 1)));
        }
    }
}
