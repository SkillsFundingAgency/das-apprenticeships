using AutoFixture;
using FluentAssertions;
using Moq;
using SFA.DAS.Learning.DataTransferObjects;
using SFA.DAS.Learning.Domain.Repositories;
using SFA.DAS.Learning.Queries.GetPendingPriceChange;

namespace SFA.DAS.Learning.Queries.UnitTests;

public class WhenGetPendingPriceChange
{
    private Fixture _fixture;
    private Mock<IApprenticeshipQueryRepository> _apprenticeshipQueryRepository;
    private GetPendingPriceChangeQueryHandler _sut;

    [SetUp]
    public void Setup()
    {

        _fixture = new Fixture();
        _apprenticeshipQueryRepository = new Mock<IApprenticeshipQueryRepository>();
        _sut = new GetPendingPriceChangeQueryHandler(_apprenticeshipQueryRepository.Object);
    }

    [Test]
    public async Task ThenWhenPendingPriceChangeExists_PendingPriceIsReturned()
    {
        //Arrange
        var query = _fixture.Create<GetPendingPriceChangeRequest>();
        var expectedResult = _fixture.Create<PendingPriceChange>();

        _apprenticeshipQueryRepository
            .Setup(x => x.GetPendingPriceChange(query.ApprenticeshipKey))
            .ReturnsAsync(expectedResult);

        //Act
        var actualResult = await _sut.Handle(query);

        //Assert
        actualResult.HasPendingPriceChange.Should().BeTrue();
        actualResult.PendingPriceChange.Should().Be(expectedResult);
    }
    
    [Test]
    public async Task ThenWhenPendingPriceChangeDoesNotExist_PendingPriceChangeIsNotReturned()
    {
        //Arrange
        var query = _fixture.Create<GetPendingPriceChangeRequest>();

        _apprenticeshipQueryRepository
            .Setup(x => x.GetPendingPriceChange(query.ApprenticeshipKey))
            .ReturnsAsync((PendingPriceChange)null!);

        //Act
        var actualResult = await _sut.Handle(query);

        //Assert
        actualResult.HasPendingPriceChange.Should().BeFalse();
        actualResult.PendingPriceChange.Should().BeNull();
    }
}