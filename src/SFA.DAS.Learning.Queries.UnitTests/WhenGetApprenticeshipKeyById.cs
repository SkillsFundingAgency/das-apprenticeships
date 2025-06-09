using AutoFixture;
using FluentAssertions;
using Moq;
using SFA.DAS.Learning.Domain.Repositories;
using SFA.DAS.Learning.Queries.GetLearningKeyByLearningId;

namespace SFA.DAS.Learning.Queries.UnitTests;

public class WhenGetApprenticeshipKeyById
{
    private Fixture _fixture;
    private Mock<IApprenticeshipQueryRepository> _apprenticeshipQueryRepository;
    private GetLearningKeyByLearningIdQueryHandler _sut;

    [SetUp]
    public void Setup()
    {

        _fixture = new Fixture();
        _apprenticeshipQueryRepository = new Mock<IApprenticeshipQueryRepository>();
        _sut = new GetLearningKeyByLearningIdQueryHandler(_apprenticeshipQueryRepository.Object);
    }

    [Test]
    public async Task ThenApprenticeshipKeyIsReturned()
    {
        //Arrange
        var query = _fixture.Create<GetLearningKeyByLearningIdRequest>();
        var expectedResult = _fixture.Create<GetLearningKeyByLearningIdResponse>();

        _apprenticeshipQueryRepository
            .Setup(x => x.GetKeyByApprenticeshipId(query.ApprenticeshipId))
            .ReturnsAsync(expectedResult.ApprenticeshipKey);

        //Act
        var actualResult = await _sut.Handle(query);

        //Assert
        actualResult.Should().BeEquivalentTo(expectedResult);
    }
}