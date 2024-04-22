using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Apprenticeships.InnerApi.Controllers;
using SFA.DAS.Apprenticeships.Queries;
using SFA.DAS.Apprenticeships.Queries.GetApprenticeshipKey;

namespace SFA.DAS.Apprenticeships.InnerApi.UnitTests.Controllers.ApprenticeshipControllerTests;

public class WhenGetApprenticeshipKey
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
        _sut = new ApprenticeshipController(_queryDispatcher.Object, _mockLogger.Object);
    }

    [Test]
    public async Task ThenApprenticeshipPriceIsReturned()
    {
        var apprenticeshipHashedId = _fixture.Create<string>();
        var expectedResult = _fixture.Create<Guid>();

        _queryDispatcher
            .Setup(x => x.Send<GetApprenticeshipKeyRequest, GetApprenticeshipKeyResponse>(It.Is<GetApprenticeshipKeyRequest>(r => r.ApprenticeshipHashedId == apprenticeshipHashedId)))
            .ReturnsAsync(new GetApprenticeshipKeyResponse{ ApprenticeshipKey = expectedResult });

        var result = await _sut.GetApprenticeshipKey(apprenticeshipHashedId);

        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.Value.Should().Be(expectedResult);
    }
}