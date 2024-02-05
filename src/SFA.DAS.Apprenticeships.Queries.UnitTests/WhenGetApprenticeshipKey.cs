using AutoFixture;
using FluentAssertions;
using Moq;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Queries.GetApprenticeshipKey;

namespace SFA.DAS.Apprenticeships.Queries.UnitTests;

public class WhenGetApprenticeshipKey
{
    private Fixture _fixture;
    private Mock<IApprenticeshipQueryRepository> _apprenticeshipQueryRepository;
    private GetApprenticeshipKeyRequestQueryHandler _sut;

    [SetUp]
    public void Setup()
    {

        _fixture = new Fixture();
        _apprenticeshipQueryRepository = new Mock<IApprenticeshipQueryRepository>();
        _sut = new GetApprenticeshipKeyRequestQueryHandler(_apprenticeshipQueryRepository.Object);
    }

    [Test]
    public async Task ThenApprenticeshipPriceIsReturned()
    {
        //Arrange
        var query = _fixture.Create<GetApprenticeshipKeyRequest>();
        var expectedResult = _fixture.Create<GetApprenticeshipKeyResponse>();

        _apprenticeshipQueryRepository
            .Setup(x => x.GetKey(query.ApprenticeshipHashedId))
            .ReturnsAsync(expectedResult.ApprenticeshipKey);

        //Act
        var actualResult = await _sut.Handle(query);

        //Assert
        actualResult.Should().BeEquivalentTo(expectedResult);
    }
}