using AutoFixture;
using FluentAssertions;
using Moq;
using SFA.DAS.Apprenticeships.DataTransferObjects;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Queries.GetApprenticeshipsWithEpisodes;

namespace SFA.DAS.Apprenticeships.Queries.UnitTests;

public class WhenGetApprenticeshipsWithEpisodes
{
    private Fixture _fixture;
    private Mock<IApprenticeshipQueryRepository> _apprenticeshipQueryRepository;
    private GetApprenticeshipsWithEpisodesRequestQueryHandler _sut;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _apprenticeshipQueryRepository = new Mock<IApprenticeshipQueryRepository>();
        _sut = new GetApprenticeshipsWithEpisodesRequestQueryHandler(_apprenticeshipQueryRepository.Object);
    }

    [Test]
    public async Task ThenApprenticeshipsWithEpisodesAreReturned()
    {
        // Arrange
        var query = _fixture.Create<GetApprenticeshipsWithEpisodesRequest>();
        var apprenticeships = _fixture.Create<List<ApprenticeshipWithEpisodes>>();
        var expectedResponse = new GetApprenticeshipsWithEpisodesResponse(query.Ukprn, apprenticeships);

        _apprenticeshipQueryRepository
            .Setup(x => x.GetApprenticeshipsWithEpisodes(query.Ukprn))
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
        var query = _fixture.Create<GetApprenticeshipsWithEpisodesRequest>();

        _apprenticeshipQueryRepository
            .Setup(x => x.GetApprenticeshipsWithEpisodes(query.Ukprn))
            .ReturnsAsync((List<ApprenticeshipWithEpisodes>?)null);

        // Act
        var actualResult = await _sut.Handle(query);

        // Assert
        actualResult.Should().BeNull();
    }
}