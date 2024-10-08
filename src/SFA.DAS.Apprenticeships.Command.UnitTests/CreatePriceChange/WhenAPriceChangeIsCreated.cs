﻿using System;
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

            if(expectAutoApprove)
				_apprenticeshipRepository.Verify(x => x.Update(It.Is<ApprenticeshipDomainModel>(y =>
					y.GetEntity().PriceHistories.Single().PriceChangeRequestStatus == ChangeRequestStatus.Approved)));
            else
	            _apprenticeshipRepository.Verify(x => x.Update(It.Is<ApprenticeshipDomainModel>(y =>
		            y.GetEntity().PriceHistories.Single().PriceChangeRequestStatus == ChangeRequestStatus.Approved)), Times.Never);
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
    }
}
