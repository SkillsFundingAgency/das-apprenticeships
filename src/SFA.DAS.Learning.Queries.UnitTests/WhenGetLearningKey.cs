using AutoFixture;
using FluentAssertions;
using Moq;
using SFA.DAS.Learning.Domain.Repositories;
using SFA.DAS.Learning.Queries.GetLearningKey;

namespace SFA.DAS.Learning.Queries.UnitTests;

public class WhenGetLearningKey
{
    private Fixture _fixture;
    private Mock<ILearningQueryRepository> _apprenticeshipQueryRepository;
    private GetLearningKeyRequestQueryHandler _sut;

    [SetUp]
    public void Setup()
    {

        _fixture = new Fixture();
        _apprenticeshipQueryRepository = new Mock<ILearningQueryRepository>();
        _sut = new GetLearningKeyRequestQueryHandler(_apprenticeshipQueryRepository.Object);
    }

    [Test]
    public async Task ThenApprenticeshipKeyIsReturned()
    {
        //Arrange
        var query = _fixture.Create<GetLearningKeyRequest>();
        var expectedResult = _fixture.Create<GetLearningKeyResponse>();

        _apprenticeshipQueryRepository
            .Setup(x => x.GetKey(query.ApprenticeshipHashedId))
            .ReturnsAsync(expectedResult.ApprenticeshipKey);

        //Act
        var actualResult = await _sut.Handle(query);

        //Assert
        actualResult.Should().BeEquivalentTo(expectedResult);
    }
}