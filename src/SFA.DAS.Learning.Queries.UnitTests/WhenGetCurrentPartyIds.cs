using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Learning.DataTransferObjects;
using SFA.DAS.Learning.Domain.Repositories;
using SFA.DAS.Learning.Queries.GetCurrentPartyIds;

namespace SFA.DAS.Learning.Queries.UnitTests;

public class WhenGetCurrentPartyIds
{
    private Fixture _fixture;
    private Mock<IApprenticeshipQueryRepository> _apprenticeshipQueryRepository;
    private GetCurrentPartyIdsRequestQueryHandler _sut;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _apprenticeshipQueryRepository = new Mock<IApprenticeshipQueryRepository>();
        _sut = new GetCurrentPartyIdsRequestQueryHandler(_apprenticeshipQueryRepository.Object, Mock.Of<ILogger<GetCurrentPartyIdsRequestQueryHandler>>());
    }

    [Test]
    public async Task ThenCurrentPartyIdsResponseIsReturned()
    {
        // Arrange
        var query = _fixture.Create<GetCurrentPartyIdsRequest>();
        var currentPartyIds = _fixture.Create<CurrentPartyIds>();
        var expectedResponse = new GetCurrentPartyIdsResponse(currentPartyIds.Ukprn, currentPartyIds.EmployerAccountId, currentPartyIds.ApprovalsApprenticeshipId);

        _apprenticeshipQueryRepository
            .Setup(x => x.GetCurrentPartyIds(query.ApprenticeshipKey))
            .ReturnsAsync(currentPartyIds);

        // Act
        var actualResult = await _sut.Handle(query);

        // Assert
        actualResult.Should().BeEquivalentTo(expectedResponse);
    }

    [Test]
    public async Task ThenNullIsReturnedWhenNoCurrentPartyIdsFound()
    {
        // Arrange
        var query = _fixture.Create<GetCurrentPartyIdsRequest>();

        _apprenticeshipQueryRepository
            .Setup(x => x.GetCurrentPartyIds(query.ApprenticeshipKey))
            .ReturnsAsync((CurrentPartyIds?)null);

        // Act
        var actualResult = await _sut.Handle(query);

        // Assert
        actualResult.Should().BeNull();
    }
}