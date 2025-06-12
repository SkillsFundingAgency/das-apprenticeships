using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Learning.Command;
using SFA.DAS.Learning.InnerApi.Controllers;
using SFA.DAS.Learning.InnerApi.Services;
using SFA.DAS.Learning.Queries;
using SFA.DAS.Learning.Queries.GetLearningKey;

namespace SFA.DAS.Learning.InnerApi.UnitTests.Controllers.ApprenticeshipControllerTests;

public class WhenGetApprenticeshipKey
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
    public async Task ThenApprenticeshipPriceIsReturned()
    {
        var apprenticeshipHashedId = _fixture.Create<string>();
        var expectedResult = _fixture.Create<Guid>();

        _queryDispatcher
            .Setup(x => x.Send<GetLearningKeyRequest, GetLearningKeyResponse>(It.Is<GetLearningKeyRequest>(r => r.ApprenticeshipHashedId == apprenticeshipHashedId)))
            .ReturnsAsync(new GetLearningKeyResponse{ LearningKey = expectedResult });

        var result = await _sut.GetLearningKey(apprenticeshipHashedId);

        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.Value.Should().Be(expectedResult);
    }
}