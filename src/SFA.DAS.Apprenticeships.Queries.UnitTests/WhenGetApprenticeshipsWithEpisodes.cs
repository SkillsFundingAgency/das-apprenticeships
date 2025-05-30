﻿using AutoFixture;
using Castle.Core.Logging;
using FluentAssertions;
using Microsoft.Extensions.Logging;
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
        _sut = new GetApprenticeshipsWithEpisodesRequestQueryHandler(_apprenticeshipQueryRepository.Object, Mock.Of<ILogger<GetApprenticeshipsWithEpisodesRequestQueryHandler>>());
    }

    [Test]
    public async Task ThenApprenticeshipsWithEpisodesAreReturned()
    {
        // Arrange
        var query = _fixture.Create<GetApprenticeshipsWithEpisodesRequest>();
        query.CollectionYear = 2021;
        query.CollectionPeriod = 1;
        var apprenticeships = _fixture.Create<List<ApprenticeshipWithEpisodes>>();
        var expectedResponse = new GetApprenticeshipsWithEpisodesResponse(query.Ukprn, apprenticeships);

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
        var query = _fixture.Create<GetApprenticeshipsWithEpisodesRequest>();
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