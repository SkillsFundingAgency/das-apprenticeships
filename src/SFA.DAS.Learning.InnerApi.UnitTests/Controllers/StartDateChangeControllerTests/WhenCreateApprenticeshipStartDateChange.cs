using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Learning.Command;
using SFA.DAS.Learning.Command.CreateStartDateChange;
using SFA.DAS.Learning.InnerApi.Controllers;
using SFA.DAS.Learning.InnerApi.Requests;
using SFA.DAS.Learning.Queries;

namespace SFA.DAS.Learning.InnerApi.UnitTests.Controllers.StartDateChangeControllerTests
{
    public class WhenCreateApprenticeshipStartDateChange
    {
        private Fixture _fixture;
        private Mock<ICommandDispatcher> _commandDispatcher;
        private StartDateChangeController _sut;
        private Mock<ILogger<StartDateChangeController>> _logger;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _commandDispatcher = new Mock<ICommandDispatcher>();
            _logger = new Mock<ILogger<StartDateChangeController>>();
            _sut = new StartDateChangeController(Mock.Of<IQueryDispatcher>(), _commandDispatcher.Object, _logger.Object);
        }

        [Test]
        public async Task ThenOkResultIsReturned()
        {
            // Arrange
            var apprenticeshipKey = _fixture.Create<Guid>();
            var request = _fixture.Create<PostCreateApprenticeshipStartDateChangeRequest>();

            // Act
            var result = await _sut.CreateApprenticeshipStartDateChange(apprenticeshipKey, request);

            // Assert
            result.Should().BeOfType<OkResult>();

            _commandDispatcher.Verify(x => x.Send<CreateStartDateChangeCommand>(It.Is<CreateStartDateChangeCommand>(r =>
                r.Initiator == request.Initiator &&
                r.ApprenticeshipKey == apprenticeshipKey &&
                r.UserId == request.UserId &&
                r.ActualStartDate == request.ActualStartDate &&
                r.PlannedEndDate == request.PlannedEndDate &&
                r.Reason == request.Reason
            ), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task ThenBadRequestIsReturnedWhenInvalidRequest()
        {
            // Arrange
            var apprenticeshipKey = _fixture.Create<Guid>();
            var request = _fixture.Create<PostCreateApprenticeshipStartDateChangeRequest>();
            _commandDispatcher.Setup(x => x.Send<CreateStartDateChangeCommand>(It.IsAny<CreateStartDateChangeCommand>(), It.IsAny<CancellationToken>())).Throws<ArgumentException>();

            // Act
            var result = await _sut.CreateApprenticeshipStartDateChange(apprenticeshipKey, request);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }
    }
}
