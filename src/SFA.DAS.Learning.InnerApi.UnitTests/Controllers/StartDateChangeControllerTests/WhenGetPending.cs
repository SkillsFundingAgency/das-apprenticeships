using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Learning.Command;
using SFA.DAS.Learning.InnerApi.Controllers;
using SFA.DAS.Learning.Queries;
using SFA.DAS.Learning.Queries.GetPendingStartDateChange;

namespace SFA.DAS.Learning.InnerApi.UnitTests.Controllers.StartDateChangeControllerTests;

public class WhenGetPending
{
    private Fixture _fixture;
    private Mock<IQueryDispatcher> _queryDispatcher;
    private StartDateChangeController _sut;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _queryDispatcher = new Mock<IQueryDispatcher>();
        _sut = new StartDateChangeController(_queryDispatcher.Object, Mock.Of<ICommandDispatcher>(),
            Mock.Of<ILogger<StartDateChangeController>>());
    }

    [Test]
    public async Task ThenPendingStartDateChangeIsReturned()
    {
        // Arrange
        var apprenticeshipKey = _fixture.Create<Guid>();
        var expectedResult = _fixture.Create<GetPendingStartDateChangeResponse>();

        _queryDispatcher
            .Setup(x => x.Send<GetPendingStartDateChangeRequest, GetPendingStartDateChangeResponse>(
                It.Is<GetPendingStartDateChangeRequest>(r => r.ApprenticeshipKey == apprenticeshipKey)))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _sut.GetPendingStartDateChange(apprenticeshipKey);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.Value.Should().Be(expectedResult);
    }

    [Test]
    public async Task ThenNotFoundIsReturnedIfNoPendingStartDateChange()
    {
        // Arrange
        var apprenticeshipKey = _fixture.Create<Guid>();
        var expectedResult = new GetPendingStartDateChangeResponse { HasPendingStartDateChange = false };

        _queryDispatcher
            .Setup(x => x.Send<GetPendingStartDateChangeRequest, GetPendingStartDateChangeResponse>(
                It.Is<GetPendingStartDateChangeRequest>(r => r.ApprenticeshipKey == apprenticeshipKey)))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _sut.GetPendingStartDateChange(apprenticeshipKey);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }
}