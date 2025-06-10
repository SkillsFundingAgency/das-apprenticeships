using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Learning.Command;
using SFA.DAS.Learning.InnerApi.Controllers;
using SFA.DAS.Learning.InnerApi.Services;
using SFA.DAS.Learning.Queries;
using SFA.DAS.Learning.Queries.GetLearnerStatus;

namespace SFA.DAS.Learning.InnerApi.UnitTests.Controllers.ApprenticeshipControllerTests;

public class WhenGetLearnerStatus
{
    private Fixture _fixture;
    private Mock<IQueryDispatcher> _queryDispatcher;
    private Mock<ICommandDispatcher> _commandDispatcher;
    private Mock<ILogger<LearningController>> _mockLogger;
    private LearningController _sut;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _queryDispatcher = new Mock<IQueryDispatcher>();
        _commandDispatcher = new Mock<ICommandDispatcher>();
        _mockLogger = new Mock<ILogger<LearningController>>();
        _sut = new LearningController(_queryDispatcher.Object, _commandDispatcher.Object, _mockLogger.Object, Mock.Of<IPagedLinkHeaderService>());
    }

    [Test]
    public async Task ThenLearnerStatusIsReturned()
    {
        var apprenticeshipKey = _fixture.Create<Guid>();
        var expectedResult = _fixture.Create<GetLearnerStatusResponse>();

        _queryDispatcher
            .Setup(x => x.Send<GetLearnerStatusRequest, GetLearnerStatusResponse>(It.Is<GetLearnerStatusRequest>(r => r.ApprenticeshipKey == apprenticeshipKey)))
            .ReturnsAsync(expectedResult);

        var result = await _sut.GetLearnerStatus(apprenticeshipKey);

        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.Value.Should().Be(expectedResult);
    }

    [Test]
    public async Task ThenNotFoundIsReturnedWhenNoRecordExists()
    {
        var apprenticeshipKey = _fixture.Create<Guid>(); ;

        _queryDispatcher
            .Setup(x => x.Send<GetLearnerStatusRequest, GetLearnerStatusResponse>(It.Is<GetLearnerStatusRequest>(r => r.ApprenticeshipKey == apprenticeshipKey)))
            .ReturnsAsync((GetLearnerStatusResponse)null!);

        var result = await _sut.GetLearnerStatus(apprenticeshipKey);

        result.Should().BeOfType<NotFoundResult>();
    }
}