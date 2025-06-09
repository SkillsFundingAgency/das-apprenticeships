using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Learning.DataTransferObjects;
using SFA.DAS.Learning.Domain.Repositories;
using SFA.DAS.Learning.Queries.GetLearningsWithEpisodes;

namespace SFA.DAS.Learning.Queries.UnitTests;

public class WhenGetApprenticeshipsWithEpisodes
{
    private Fixture _fixture;
    private Mock<IApprenticeshipQueryRepository> _apprenticeshipQueryRepository;
    private GetLearningsWithEpisodesRequestQueryHandler _sut;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _apprenticeshipQueryRepository = new Mock<IApprenticeshipQueryRepository>();
        _sut = new GetLearningsWithEpisodesRequestQueryHandler(_apprenticeshipQueryRepository.Object, Mock.Of<ILogger<GetLearningsWithEpisodesRequestQueryHandler>>());
    }

    [Test]
    public async Task ThenApprenticeshipsWithEpisodesAreReturned()
    {
        // Arrange
        var query = _fixture.Create<GetLearningsWithEpisodesRequest>();
        query.CollectionYear = 2021;
        query.CollectionPeriod = 1;
        var apprenticeships = _fixture.Create<List<ApprenticeshipWithEpisodes>>();
        var expectedResponse = new GetLearningsWithEpisodesResponse(query.Ukprn, apprenticeships);

        _apprenticeshipQueryRepository
            .Setup(x => x.GetApprenticeshipsWithEpisodes(query.Ukprn, It.IsAny<DateTime>()))
            .ReturnsAsync(apprenticeships);

        // Act
        var actualResult = await _sut.Handle(query);

        // Assert
        actualResult.Should().BeEquivalentTo(expectedResponse);
    }

    [Test]
    public async Task ThenNullIsReturnedWhenNoApprenticeshipsWithEpisodesExist()
    {
        // Arrange
        var query = _fixture.Create<GetLearningsWithEpisodesRequest>();
        query.CollectionYear = 2021;
        query.CollectionPeriod = 1;

        _apprenticeshipQueryRepository
            .Setup(x => x.GetApprenticeshipsWithEpisodes(query.Ukprn, It.IsAny<DateTime>()))
            .ReturnsAsync((List<ApprenticeshipWithEpisodes>?)null);

        // Act
        var actualResult = await _sut.Handle(query);

        // Assert
        actualResult.Should().BeNull();
    }
}