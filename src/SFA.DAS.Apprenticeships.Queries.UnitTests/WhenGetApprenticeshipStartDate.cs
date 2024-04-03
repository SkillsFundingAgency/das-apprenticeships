using AutoFixture;
using FluentAssertions;
using Moq;
using SFA.DAS.Apprenticeships.DataTransferObjects;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Queries.GetApprenticeshipStartDate;

namespace SFA.DAS.Apprenticeships.Queries.UnitTests;

public class WhenGetApprenticeshipStartDate
{
    private Fixture _fixture;
    private Mock<IApprenticeshipQueryRepository> _apprenticeshipQueryRepository;
    private GetApprenticeshipStartDateQueryHandler _sut;

    [SetUp]
    public void Setup()
    {

        _fixture = new Fixture();
        _apprenticeshipQueryRepository = new Mock<IApprenticeshipQueryRepository>();
        _sut = new GetApprenticeshipStartDateQueryHandler(_apprenticeshipQueryRepository.Object);
    }

    [Test]
    public async Task ThenApprenticeshipStartDateIsReturned()
    {
        //Arrange
        var query = _fixture.Create<GetApprenticeshipStartDateRequest>();
        var expectedResult = _fixture.Create<GetApprenticeshipStartDateResponse>();

        _apprenticeshipQueryRepository
            .Setup(x => x.GetStartDate(query.ApprenticeshipKey))
            .ReturnsAsync(expectedResult.ApprenticeshipStartDate);

        //Act
        var actualResult = await _sut.Handle(query);

        //Assert
        actualResult.Should().BeEquivalentTo(expectedResult);
    }
    
    [Test]
    public async Task ThenNullIsReturnedWhenNoRecordExists()
    {
        //Arrange
        var query = _fixture.Create<GetApprenticeshipStartDateRequest>();

        _apprenticeshipQueryRepository
            .Setup(x => x.GetStartDate(query.ApprenticeshipKey))
            .ReturnsAsync((ApprenticeshipStartDate)null!);

        //Act
        var actualResult = await _sut.Handle(query);

        //Assert
        actualResult!.ApprenticeshipStartDate.Should().BeNull();
    }
}