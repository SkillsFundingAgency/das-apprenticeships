using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Learning.Command;
using SFA.DAS.Learning.InnerApi.Controllers;
using SFA.DAS.Learning.InnerApi.Services;
using SFA.DAS.Learning.Queries;
using SFA.DAS.Learning.Queries.GetApprenticeshipsWithEpisodes;

namespace SFA.DAS.Apprenticeships.InnerApi.UnitTests.Controllers.ApprenticeshipControllerTests;

public class WhenGetApprenticeships
{
    private Fixture _fixture;
    private Mock<IQueryDispatcher> _queryDispatcher;
    private Mock<ILogger<ApprenticeshipController>> _mockLogger;
    private ApprenticeshipController _sut;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _queryDispatcher = new Mock<IQueryDispatcher>();
        _mockLogger = new Mock<ILogger<ApprenticeshipController>>();
        _sut = new ApprenticeshipController(_queryDispatcher.Object, Mock.Of<ICommandDispatcher>(), Mock.Of<ILogger<ApprenticeshipController>>(), Mock.Of<IPagedLinkHeaderService>());
    }

    [Test]
    public async Task ThenApprenticeshipsAreReturned()
    {
        // Arrange
        var ukprn = _fixture.Create<long>();
        var collectionYear = _fixture.Create<short>();
        var collectionPeriod = _fixture.Create<byte>();
        var expectedResponse = _fixture.Create<GetApprenticeshipsWithEpisodesResponse>();

        _queryDispatcher
            .Setup(x => x.Send<GetApprenticeshipsWithEpisodesRequest, GetApprenticeshipsWithEpisodesResponse?>(It.Is<GetApprenticeshipsWithEpisodesRequest>(r => r.Ukprn == ukprn)))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _sut.GetApprenticeshipsForFm36(ukprn, collectionYear, collectionPeriod);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.Value.Should().Be(expectedResponse);
    }

    [Test]
    public async Task ThenNotFoundIsReturnedWhenNoRecordExists()
    {
        // Arrange
        var ukprn = _fixture.Create<long>();
        var collectionYear = _fixture.Create<short>();
        var collectionPeriod = _fixture.Create<byte>();

        _queryDispatcher
            .Setup(x => x.Send<GetApprenticeshipsWithEpisodesRequest, GetApprenticeshipsWithEpisodesResponse?>(It.Is<GetApprenticeshipsWithEpisodesRequest>(r => r.Ukprn == ukprn)))
            .ReturnsAsync((GetApprenticeshipsWithEpisodesResponse?)null);

        // Act
        var result = await _sut.GetApprenticeshipsForFm36(ukprn, collectionYear, collectionPeriod);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}