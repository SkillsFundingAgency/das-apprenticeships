using System;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Command.CreateStartDateChange;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;

namespace SFA.DAS.Apprenticeships.Command.UnitTests.CreateStartDateChange
{
    [TestFixture]
    public class WhenAStartDateChangeIsCreated
    {
        private CreateStartDateChangeCommandHandler _commandHandler;
        private Mock<IApprenticeshipRepository> _apprenticeshipRepository;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _apprenticeshipRepository = new Mock<IApprenticeshipRepository>();
            _commandHandler = new CreateStartDateChangeCommandHandler(_apprenticeshipRepository.Object);
            _fixture = new Fixture();
            _fixture.Customize(new ApprenticeshipCustomization());
        }

        [Test]
        public async Task ThenStartDateChangeIsAddedToApprenticeshipForProvider()
        {
            // Arrange
            var command = _fixture.Create<CreateStartDateChangeCommand>();
            command.Initiator = ChangeInitiator.Provider.ToString();
            var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
            _apprenticeshipRepository.Setup(x => x.Get(command.ApprenticeshipKey)).ReturnsAsync(apprenticeship);

            // Act
            await _commandHandler.Handle(command);

            // Assert
            _apprenticeshipRepository.Verify(x => x.Update(It.Is<ApprenticeshipDomainModel>(y =>
                y.GetEntity().StartDateChanges.Count == 1 &&
                y.GetEntity().StartDateChanges[0].ActualStartDate == command.ActualStartDate &&
                y.GetEntity().StartDateChanges[0].PlannedEndDate == command.PlannedEndDate &&
                y.GetEntity().StartDateChanges[0].Reason == command.Reason &&
                y.GetEntity().StartDateChanges[0].ProviderApprovedBy == command.UserId &&
                y.GetEntity().StartDateChanges[0].ProviderApprovedDate != null &&
                y.GetEntity().StartDateChanges[0].EmployerApprovedBy == null &&
                y.GetEntity().StartDateChanges[0].EmployerApprovedDate == null &&
                y.GetEntity().StartDateChanges[0].CreatedDate != DateTime.MinValue &&
                y.GetEntity().StartDateChanges[0].RequestStatus == ChangeRequestStatus.Created &&
                y.GetEntity().StartDateChanges[0].Initiator == ChangeInitiator.Provider
            )));
        }

        [Test]
        public async Task ThenStartDateChangeIsAddedToApprenticeshipForEmployer()
        {
            // Arrange
            var command = _fixture.Create<CreateStartDateChangeCommand>();
            command.Initiator = ChangeInitiator.Employer.ToString();
            var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
            _apprenticeshipRepository.Setup(x => x.Get(command.ApprenticeshipKey)).ReturnsAsync(apprenticeship);

            // Act
            await _commandHandler.Handle(command);

            // Assert
            _apprenticeshipRepository.Verify(x => x.Update(It.Is<ApprenticeshipDomainModel>(y =>
                y.GetEntity().StartDateChanges.Count == 1 &&
                y.GetEntity().StartDateChanges[0].ActualStartDate == command.ActualStartDate &&
                y.GetEntity().StartDateChanges[0].PlannedEndDate == command.PlannedEndDate &&
                y.GetEntity().StartDateChanges[0].Reason == command.Reason &&
                y.GetEntity().StartDateChanges[0].EmployerApprovedBy == command.UserId &&
                y.GetEntity().StartDateChanges[0].EmployerApprovedDate != null &&
                y.GetEntity().StartDateChanges[0].ProviderApprovedBy == null &&
                y.GetEntity().StartDateChanges[0].ProviderApprovedDate == null &&
                y.GetEntity().StartDateChanges[0].CreatedDate != DateTime.MinValue &&
                y.GetEntity().StartDateChanges[0].RequestStatus == ChangeRequestStatus.Created &&
                y.GetEntity().StartDateChanges[0].Initiator == ChangeInitiator.Employer
            )));
        }

        [Test]
        public void ThenAnExceptionIsThrownIfTheInitiatorIsNotSet()
        {
            // Arrange
            var command = _fixture.Create<CreateStartDateChangeCommand>();
            command.Initiator = string.Empty;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => _commandHandler.Handle(command));
        }
    }
}
