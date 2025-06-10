using AutoFixture;
using FluentAssertions;
using Moq;
using SFA.DAS.Learning.DataTransferObjects;
using SFA.DAS.Learning.Domain.Repositories;
using SFA.DAS.Learning.Queries.GetApprenticeshipStartDate;

namespace SFA.DAS.Learning.Queries.UnitTests;

public class WhenGetApprenticeshipStartDate
{
    private Fixture _fixture;
    private Mock<ILearningQueryRepository> _apprenticeshipQueryRepository;
    private GetLearningStartDateQueryHandler _sut;

    [SetUp]
    public void Setup()
    {

        _fixture = new Fixture();
        _apprenticeshipQueryRepository = new Mock<ILearningQueryRepository>();
        _sut = new GetLearningStartDateQueryHandler(_apprenticeshipQueryRepository.Object);
    }

    [Test]
    public async Task ThenApprenticeshipStartDateIsReturned()
    {
        //Arrange
        var query = _fixture.Create<GetLearningStartDateRequest>();
        var expectedResult = _fixture.Create<GetLearningStartDateResponse>();

        _apprenticeshipQueryRepository
            .Setup(x => x.GetStartDate(query.ApprenticeshipKey))
            .ReturnsAsync(expectedResult.LearningStartDate);

        //Act
        var actualResult = await _sut.Handle(query);

        //Assert
        actualResult.Should().BeEquivalentTo(expectedResult);
    }
    
    [Test]
    public async Task ThenNullIsReturnedWhenNoRecordExists()
    {
        //Arrange
        var query = _fixture.Create<GetLearningStartDateRequest>();

        _apprenticeshipQueryRepository
            .Setup(x => x.GetStartDate(query.ApprenticeshipKey))
            .ReturnsAsync((ApprenticeshipStartDate)null!);

        //Act
        var actualResult = await _sut.Handle(query);

        //Assert
        actualResult!.LearningStartDate.Should().BeNull();
    }
}