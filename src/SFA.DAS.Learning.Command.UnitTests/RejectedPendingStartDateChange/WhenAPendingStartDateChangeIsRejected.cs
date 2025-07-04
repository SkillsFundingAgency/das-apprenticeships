using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Learning.Command.RejectStartDateChange;
using SFA.DAS.Learning.Domain.Apprenticeship;
using SFA.DAS.Learning.Domain.Repositories;
using SFA.DAS.Learning.Enums;
using SFA.DAS.Learning.TestHelpers.AutoFixture.Customizations;

namespace SFA.DAS.Learning.Command.UnitTests.RejectedPendingStartDateChange
{
    [TestFixture]
    public class WhenAPendingStartDateChangeIsRejected
    {
        private RejectStartDateChangeCommandHandler _commandHandler;
        private Mock<ILearningRepository> _apprenticeshipRepository;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _apprenticeshipRepository = new Mock<ILearningRepository>();
            _commandHandler = new RejectStartDateChangeCommandHandler(_apprenticeshipRepository.Object);
            _fixture = new Fixture();
            _fixture.Customize(new ApprenticeshipCustomization());
        }

        [Test]
        public async Task ThenStartDateChangeIsRejected()
        {
            // Arrange
            var command = _fixture.Create<RejectStartDateChangeCommand>();
            var apprenticeship = _fixture.Create<LearningDomainModel>();
            apprenticeship.AddStartDateChange(_fixture.Create<DateTime>(), _fixture.Create<DateTime>(), _fixture.Create<string>(),
                _fixture.Create<string>(), _fixture.Create<DateTime>(), _fixture.Create<string>(),
                _fixture.Create<DateTime>(), _fixture.Create<DateTime>(), ChangeRequestStatus.Created,
                ChangeInitiator.Provider);

            _apprenticeshipRepository.Setup(x => x.Get(command.ApprenticeshipKey)).ReturnsAsync(apprenticeship);

            // Act
            await _commandHandler.Handle(command);

            // Assert
            _apprenticeshipRepository.Verify(x => x.Update(It.Is<LearningDomainModel>(y => y.GetEntity().StartDateChanges.Count(z => z.RequestStatus == ChangeRequestStatus.Rejected) == 1)));
        }
    }
}