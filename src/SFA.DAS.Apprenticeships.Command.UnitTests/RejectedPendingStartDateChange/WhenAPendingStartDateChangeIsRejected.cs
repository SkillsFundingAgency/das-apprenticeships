using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Command.RejectStartDateChange;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;

namespace SFA.DAS.Apprenticeships.Command.UnitTests.RejectedPendingStartDateChange
{
    [TestFixture]
    public class WhenAPendingStartDateChangeIsRejected
    {
        private RejectStartDateChangeCommandHandler _commandHandler;
        private Mock<IApprenticeshipRepository> _apprenticeshipRepository;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _apprenticeshipRepository = new Mock<IApprenticeshipRepository>();
            _commandHandler = new RejectStartDateChangeCommandHandler(_apprenticeshipRepository.Object);
            _fixture = new Fixture();
            _fixture.Customize(new ApprenticeshipCustomization());
        }

        [Test]
        public async Task ThenStartDateChangeIsRejected()
        {
            // Arrange
            var command = _fixture.Create<RejectStartDateChangeCommand>();
            var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
            apprenticeship.AddStartDateChange(_fixture.Create<DateTime>(), _fixture.Create<string>(),
                _fixture.Create<string>(), _fixture.Create<DateTime>(), _fixture.Create<string>(),
                _fixture.Create<DateTime>(), _fixture.Create<DateTime>(), ChangeRequestStatus.Created,
                ChangeInitiator.Provider);

            _apprenticeshipRepository.Setup(x => x.Get(command.ApprenticeshipKey)).ReturnsAsync(apprenticeship);

            // Act
            await _commandHandler.Handle(command);

            // Assert
            _apprenticeshipRepository.Verify(x => x.Update(It.Is<ApprenticeshipDomainModel>(y => y.GetEntity().StartDateChanges.Count(z => z.RequestStatus == ChangeRequestStatus.Rejected) == 1)));
        }
    }
}