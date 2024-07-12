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
    public class WhenAPriceChangeIsApproved
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
        public async Task ByEmployerThenThePriceHistoryIsApproved()
        {
            //Arrange
            var command = _fixture.Create<ApprovePriceChangeCommand>();
            command.AssessmentPrice = null;
            command.TrainingPrice = null;
            var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
            ApprenticeshipDomainModelTestHelper.AddEpisode(apprenticeship);
            ApprenticeshipDomainModelTestHelper.AddPendingPriceChangeProviderInitiated(apprenticeship);
            _apprenticeshipRepository.Setup(x => x.Get(command.ApprenticeshipKey)).ReturnsAsync(apprenticeship);

            //Act
            await _commandHandler.Handle(command);
            
            //Assert
            _apprenticeshipRepository.Verify(x => x.Update(
                It.Is<ApprenticeshipDomainModel>(y => y
                        .GetEntity()
                        .PriceHistories
                        .Count(z => z.PriceChangeRequestStatus == ChangeRequestStatus.Approved 
                                    && z.EmployerApprovedBy == command.UserId
                                    && z.EmployerApprovedBy != null) == 1)));
        }

        [Test]
        public async Task ByProviderThenThePriceHistoryIsApproved()
        {
            //Arrange
            var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
            var command = _fixture.Create<ApprovePriceChangeCommand>();
            var totalPrice = command.TrainingPrice!.Value + command.AssessmentPrice!.Value;
            ApprenticeshipDomainModelTestHelper.AddEpisode(apprenticeship);
            ApprenticeshipDomainModelTestHelper.AddPendingPriceChangeEmployerInitiated(
                apprenticeship, 
                totalPrice, 
                effectiveFromDate:apprenticeship.LatestPrice.StartDate.AddDays(_fixture.Create<int>()));
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
