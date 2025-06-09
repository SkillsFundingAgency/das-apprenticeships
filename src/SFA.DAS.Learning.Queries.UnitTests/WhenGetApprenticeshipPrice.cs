using AutoFixture;
using FluentAssertions;
using Moq;
using SFA.DAS.Learning.DataTransferObjects;
using SFA.DAS.Learning.Domain.Repositories;
using SFA.DAS.Learning.Queries.GetApprenticeshipPrice;

namespace SFA.DAS.Apprenticeships.Queries.UnitTests;

public class WhenGetApprenticeshipPrice
{
    private Fixture _fixture;
    private Mock<IApprenticeshipQueryRepository> _apprenticeshipQueryRepository;
    private GetApprenticeshipPriceRequestQueryHandler _sut;

    [SetUp]
    public void Setup()
    {

        _fixture = new Fixture();
        _apprenticeshipQueryRepository = new Mock<IApprenticeshipQueryRepository>();
        _sut = new GetApprenticeshipPriceRequestQueryHandler(_apprenticeshipQueryRepository.Object);
    }

    [Test]
    public async Task ThenApprenticeshipPriceIsReturned()
    {
        //Arrange
        var query = _fixture.Create<GetApprenticeshipPriceRequest>();
        var expectedResult = _fixture.Create<ApprenticeshipPrice>();

        _apprenticeshipQueryRepository
            .Setup(x => x.GetPrice(query.ApprenticeshipKey))
            .ReturnsAsync(expectedResult);

        //Act
        var actualResult = await _sut.Handle(query);

        //Assert
        actualResult.Should().BeEquivalentTo(expectedResult);
    }
    
    [Test]
    public async Task ThenNullIsReturnedWhenNoRecordExists()
    {
        //Arrange
        var query = _fixture.Create<GetApprenticeshipPriceRequest>();

        _apprenticeshipQueryRepository
            .Setup(x => x.GetPrice(query.ApprenticeshipKey))
            .ReturnsAsync((ApprenticeshipPrice)null!);

        //Act
        var actualResult = await _sut.Handle(query);

        //Assert
        actualResult.Should().BeNull();
    }
}