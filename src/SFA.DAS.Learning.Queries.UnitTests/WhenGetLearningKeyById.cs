using AutoFixture;
using FluentAssertions;
using Moq;
using SFA.DAS.Learning.Domain.Repositories;
using SFA.DAS.Learning.Queries.GetLearningKeyByLearningId;

namespace SFA.DAS.Learning.Queries.UnitTests;

public class WhenGetLearningKeyById
{
    private Fixture _fixture;
    private Mock<ILearningQueryRepository> _apprenticeshipQueryRepository;
    private GetLearningKeyByLearningIdQueryHandler _sut;

    [SetUp]
    public void Setup()
    {

        _fixture = new Fixture();
        _apprenticeshipQueryRepository = new Mock<ILearningQueryRepository>();
        _sut = new GetLearningKeyByLearningIdQueryHandler(_apprenticeshipQueryRepository.Object);
    }

    [Test]
    public async Task ThenLearningKeyIsReturned()
    {
        //Arrange
        var query = _fixture.Create<GetLearningKeyByLearningIdRequest>();
        var expectedResult = _fixture.Create<GetLearningKeyByLearningIdResponse>();

        _apprenticeshipQueryRepository
            .Setup(x => x.GetKeyByLearningId(query.ApprenticeshipId))
            .ReturnsAsync(expectedResult.LearningKey);

        //Act
        var actualResult = await _sut.Handle(query);

        //Assert
        actualResult.Should().BeEquivalentTo(expectedResult);
    }
}