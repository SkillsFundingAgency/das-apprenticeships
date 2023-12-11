using AutoFixture;
using FluentAssertions;
using Moq;
using SFA.DAS.Apprenticeships.DataTransferObjects;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Queries.GetPendingPriceChange;

namespace SFA.DAS.Apprenticeships.Queries.UnitTests;

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
        var expectedResult = _fixture.Create<ApprenticeshipPrice>();

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
            .Setup(x => x.GetPrice(query.ApprenticeshipKey))
            .ReturnsAsync((ApprenticeshipPrice)null!);

        //Act
        var actualResult = await _sut.Handle(query);

        //Assert
        actualResult.HasPendingPriceChange.Should().BeFalse();
        actualResult.PendingPriceChange.Should().BeNull();
    }
}