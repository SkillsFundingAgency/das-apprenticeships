using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Apprenticeships.InnerApi.Controllers;
using SFA.DAS.Apprenticeships.Queries;
using SFA.DAS.Apprenticeships.Queries.GetApprenticeshipPaymentStatus;

namespace SFA.DAS.Apprenticeships.InnerApi.UnitTests.Controllers.ApprenticeshipControllerTests;

public class WhenGetApprenticeshipPaymentStatus
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
	public async Task ThenApprenticeshipPaymentStatusIsReturned()
	{
		var apprenticeshipKey = _fixture.Create<Guid>();
		var expectedResult = _fixture.Create<GetApprenticeshipPaymentStatusResponse>();

		_queryDispatcher
			.Setup(x => x.Send<GetApprenticeshipPaymentStatusRequest, GetApprenticeshipPaymentStatusResponse>(It.Is<GetApprenticeshipPaymentStatusRequest>(r => r.ApprenticeshipKey == apprenticeshipKey)))
			.ReturnsAsync(expectedResult);

		var result = await _sut.GetApprenticeshipPaymentStatus(apprenticeshipKey);

		result.Should().BeOfType<OkObjectResult>();
		var okResult = (OkObjectResult)result;
		okResult.Value.Should().Be(expectedResult);
	}

	[Test]
	public async Task ThenNotFoundIsReturnedWhenNoRecordExists()
	{
		var apprenticeshipKey = _fixture.Create<Guid>();

		_queryDispatcher
			.Setup(x => x.Send<GetApprenticeshipPaymentStatusRequest, GetApprenticeshipPaymentStatusResponse>(It.Is<GetApprenticeshipPaymentStatusRequest>(r => r.ApprenticeshipKey == apprenticeshipKey)))
			.ReturnsAsync((GetApprenticeshipPaymentStatusResponse)null!);

		var result = await _sut.GetApprenticeshipPaymentStatus(apprenticeshipKey);

		result.Should().BeOfType<NotFoundResult>();
	}
}