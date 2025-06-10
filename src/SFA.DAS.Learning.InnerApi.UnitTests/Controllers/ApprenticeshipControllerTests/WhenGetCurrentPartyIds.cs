using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Learning.InnerApi.Controllers;
using SFA.DAS.Learning.InnerApi.Services;
using SFA.DAS.Learning.Queries;
using SFA.DAS.Learning.Queries.GetCurrentPartyIds;

namespace SFA.DAS.Learning.InnerApi.UnitTests.Controllers.ApprenticeshipControllerTests;

public class WhenGetCurrentPartyIds
{
    private Fixture _fixture;
    private Mock<IQueryDispatcher> _queryDispatcher;
    private Mock<ILogger<LearningController>> _mockLogger;
    private LearningController _sut;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _queryDispatcher = new Mock<IQueryDispatcher>();
        _mockLogger = new Mock<ILogger<LearningController>>();
        _sut = new LearningController(_queryDispatcher.Object, null!, _mockLogger.Object, Mock.Of<IPagedLinkHeaderService>());
    }

    [Test]
    public async Task ThenCurrentPartyIdsAreReturned()
    {
        // Arrange
        var apprenticeshipKey = _fixture.Create<Guid>();
        var expectedResponse = _fixture.Create<GetCurrentPartyIdsResponse>();

        _queryDispatcher
            .Setup(x => x.Send<GetCurrentPartyIdsRequest, GetCurrentPartyIdsResponse?>(It.Is<GetCurrentPartyIdsRequest>(r => r.ApprenticeshipKey == apprenticeshipKey)))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _sut.GetCurrentPartyIds(apprenticeshipKey);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.Value.Should().Be(expectedResponse);
    }

    [Test]
    public async Task ThenNotFoundIsReturnedWhenNoCurrentPartyIds()
    {
        // Arrange
        var apprenticeshipKey = _fixture.Create<Guid>();

        _queryDispatcher
            .Setup(x => x.Send<GetCurrentPartyIdsRequest, GetCurrentPartyIdsResponse?>(It.Is<GetCurrentPartyIdsRequest>(r => r.ApprenticeshipKey == apprenticeshipKey)))
            .ReturnsAsync((GetCurrentPartyIdsResponse?)null);

        // Act
        var result = await _sut.GetCurrentPartyIds(apprenticeshipKey);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}