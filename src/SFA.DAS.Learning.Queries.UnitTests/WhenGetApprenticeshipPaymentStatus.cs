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
	private Mock<ILearningQueryRepository> _learningQueryRepository;
	private GetLearningPaymentStatusQueryHandler _sut;

	[SetUp]
	public void Setup()
	{
		_fixture = new Fixture();
		_learningQueryRepository = new Mock<ILearningQueryRepository>();
		_sut = new GetLearningPaymentStatusQueryHandler(_learningQueryRepository.Object);
	}

	[Test]
	public async Task ThenLearningPaymentStatusIsReturned()
	{
		// Arrange
		var query = _fixture.Create<GetLearningPaymentStatusRequest>();
		var queryResult = _fixture.Create<PaymentStatus>();
		var expectedResult = new GetLearningPaymentStatusResponse
		{
            LearningKey = query.LearningKey,
            PaymentsFrozen = queryResult.IsFrozen,
            ReasonFrozen = queryResult.Reason,
            FrozenOn = queryResult.FrozenOn
        };

        _learningQueryRepository
			.Setup(x => x.GetPaymentStatus(query.LearningKey))
			.ReturnsAsync(queryResult);

		// Act
		var actualResult = await _sut.Handle(query);

		// Assert
		actualResult.Should().BeEquivalentTo(expectedResult);
	}
}