using AutoFixture;
using FluentAssertions;
using Moq;
using SFA.DAS.Apprenticeships.DataTransferObjects;
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
		var queryResult = _fixture.Create<PaymentStatus>();
		var expectedResult = new GetApprenticeshipPaymentStatusResponse
		{
            ApprenticeshipKey = query.ApprenticeshipKey,
            PaymentsFrozen = queryResult.IsFrozen,
            ReasonFrozen = queryResult.Reason,
            FrozenOn = queryResult.FrozenOn
        };


        _apprenticeshipQueryRepository
			.Setup(x => x.GetPaymentStatus(query.ApprenticeshipKey))
			.ReturnsAsync(queryResult);

		// Act
		var actualResult = await _sut.Handle(query);

		// Assert
		actualResult.Should().BeEquivalentTo(expectedResult);
	}
}