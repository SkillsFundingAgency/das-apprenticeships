using AutoFixture;
using FluentAssertions;
using Moq;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Queries.GetApprenticeshipPaymentStatus;

namespace SFA.DAS.Apprenticeships.Queries.UnitTests;

public class WhenGetApprenticeshipPaymentStatus
{
	private Fixture _fixture;
	private Mock<IApprenticeshipQueryRepository> _apprenticeshipQueryRepository;
	private GetApprenticeshipPaymentStatusQueryHandler _sut;

	[SetUp]
	public void Setup()
	{
		_fixture = new Fixture();
		_apprenticeshipQueryRepository = new Mock<IApprenticeshipQueryRepository>();
		_sut = new GetApprenticeshipPaymentStatusQueryHandler(_apprenticeshipQueryRepository.Object);
	}

	[Test]
	public async Task ThenApprenticeshipPaymentStatusIsReturned()
	{
		// Arrange
		var query = _fixture.Create<GetApprenticeshipPaymentStatusRequest>();
		var expectedResult = _fixture.Create<bool>();

		_apprenticeshipQueryRepository
			.Setup(x => x.GetPaymentStatus(query.ApprenticeshipKey))
			.ReturnsAsync(expectedResult);

		// Act
		var actualResult = await _sut.Handle(query);

		// Assert
		actualResult.Should().BeEquivalentTo(new GetApprenticeshipPaymentStatusResponse { PaymentsFrozen = expectedResult });
	}
}