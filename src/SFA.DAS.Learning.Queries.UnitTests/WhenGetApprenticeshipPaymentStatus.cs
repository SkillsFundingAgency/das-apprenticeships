using AutoFixture;
using FluentAssertions;
using Moq;
using SFA.DAS.Learning.DataTransferObjects;
using SFA.DAS.Learning.Domain.Repositories;
using SFA.DAS.Learning.Queries.GetApprenticeshipPaymentStatus;

namespace SFA.DAS.Learning.Queries.UnitTests;

public class WhenGetApprenticeshipPaymentStatus
{
	private Fixture _fixture;
	private Mock<ILearningQueryRepository> _apprenticeshipQueryRepository;
	private GetLearningPaymentStatusQueryHandler _sut;

	[SetUp]
	public void Setup()
	{
		_fixture = new Fixture();
		_apprenticeshipQueryRepository = new Mock<ILearningQueryRepository>();
		_sut = new GetLearningPaymentStatusQueryHandler(_apprenticeshipQueryRepository.Object);
	}

	[Test]
	public async Task ThenApprenticeshipPaymentStatusIsReturned()
	{
		// Arrange
		var query = _fixture.Create<GetLearningPaymentStatusRequest>();
		var queryResult = _fixture.Create<PaymentStatus>();
		var expectedResult = new GetLearningPaymentStatusResponse
		{
            ApprenticeshipKey = query.LearningKey,
            PaymentsFrozen = queryResult.IsFrozen,
            ReasonFrozen = queryResult.Reason,
            FrozenOn = queryResult.FrozenOn
        };


        _apprenticeshipQueryRepository
			.Setup(x => x.GetPaymentStatus(query.LearningKey))
			.ReturnsAsync(queryResult);

		// Act
		var actualResult = await _sut.Handle(query);

		// Assert
		actualResult.Should().BeEquivalentTo(expectedResult);
	}
}