﻿using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Command.ApprovePriceChange;
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

namespace SFA.DAS.Apprenticeships.Command.UnitTests.ApprovePriceChange
{
    [TestFixture]
    public class WhenAPriceChangeIsApproved
    {
        private ApprovePriceChangeCommandHandler _commandHandler = null!;
        private Mock<IApprenticeshipRepository> _apprenticeshipRepository = null!;
        private Mock<IMessageSession> _messageSession = null!;
        private Mock<ISystemClockService> _systemClockService = null!;
        private Mock<ILogger<ApprovePriceChangeCommandHandler>> _logger = null!;
        private Fixture _fixture = null!;
        private DateTime _approvedDate = DateTime.UtcNow;

        [SetUp]
        public void SetUp()
        {
            _apprenticeshipRepository = new Mock<IApprenticeshipRepository>();
            _messageSession = new Mock<IMessageSession>();
            _systemClockService = new Mock<ISystemClockService>();
            _systemClockService.Setup(x => x.UtcNow).Returns(_approvedDate);
            _logger = new Mock<ILogger<ApprovePriceChangeCommandHandler>>();
            _commandHandler = new ApprovePriceChangeCommandHandler(
                _apprenticeshipRepository.Object, _messageSession.Object, _systemClockService.Object, _logger.Object);

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
            var effectiveFromDate = apprenticeship.LatestPrice.StartDate.AddDays(_fixture.Create<int>());
            ApprenticeshipDomainModelTestHelper.AddPendingPriceChangeProviderInitiated(apprenticeship, effectiveFromDate: effectiveFromDate);
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

            AssertEventPublished(apprenticeship, effectiveFromDate, ApprovedBy.Employer);
        }

        [Test]
        public async Task ByProviderThenThePriceHistoryIsApproved()
        {
            //Arrange
            var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
            var command = _fixture.Create<ApprovePriceChangeCommand>();
            var totalPrice = command.TrainingPrice!.Value + command.AssessmentPrice!.Value;
            ApprenticeshipDomainModelTestHelper.AddEpisode(apprenticeship);
            var effectiveFromDate = apprenticeship.LatestPrice.StartDate.AddDays(_fixture.Create<int>());

            ApprenticeshipDomainModelTestHelper.AddPendingPriceChangeEmployerInitiated(
                apprenticeship, 
                totalPrice, 
                effectiveFromDate: effectiveFromDate);
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

            AssertEventPublished(apprenticeship, effectiveFromDate, ApprovedBy.Provider);
        }

        private void AssertEventPublished(ApprenticeshipDomainModel apprenticeship, DateTime effectiveFromDate, ApprovedBy approvedBy)
        {
            _messageSession.Verify(x => x.Publish(It.Is<ApprenticeshipPriceChangedEvent>(e =>
                DoApprenticeshipDetailsMatchDomainModel(e, apprenticeship) &&
                e.ApprovedDate == _approvedDate &&
                e.ApprovedBy == approvedBy &&
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
}
