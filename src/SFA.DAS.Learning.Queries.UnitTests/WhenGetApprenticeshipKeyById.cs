using AutoFixture;
using FluentAssertions;
using Moq;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Queries.GetApprenticeshipKeyByApprenticeshipId;

namespace SFA.DAS.Apprenticeships.Queries.UnitTests;

public class WhenGetApprenticeshipKeyById
{
    private Fixture _fixture;
    private Mock<IApprenticeshipQueryRepository> _apprenticeshipQueryRepository;
    private GetApprenticeshipKeyByApprenticeshipIdQueryHandler _sut;

    [SetUp]
    public void Setup()
    {

        _fixture = new Fixture();
        _apprenticeshipQueryRepository = new Mock<IApprenticeshipQueryRepository>();
        _sut = new GetApprenticeshipKeyByApprenticeshipIdQueryHandler(_apprenticeshipQueryRepository.Object);
    }

    [Test]
    public async Task ThenApprenticeshipKeyIsReturned()
    {
        //Arrange
        var query = _fixture.Create<GetApprenticeshipKeyByApprenticeshipIdRequest>();
        var expectedResult = _fixture.Create<GetApprenticeshipKeyByApprenticeshipIdResponse>();

        _apprenticeshipQueryRepository
            .Setup(x => x.GetKeyByApprenticeshipId(query.ApprenticeshipId))
            .ReturnsAsync(expectedResult.ApprenticeshipKey);

        //Act
        var actualResult = await _sut.Handle(query);

        //Assert
        actualResult.Should().BeEquivalentTo(expectedResult);
    }
}