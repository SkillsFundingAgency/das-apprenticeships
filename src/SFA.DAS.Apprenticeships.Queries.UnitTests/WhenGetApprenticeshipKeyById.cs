using AutoFixture;
using FluentAssertions;
using Moq;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Queries.GetApprenticeshipKeyByApprenticeshipId;

namespace SFA.DAS.Apprenticeships.Queries.UnitTests;

public class WhenGetApprenticeshipKeyById
{
    private Fixture _fixture;
    private Mock<IApprovalQueryRepository> _approvalQueryRepository;
    private GetApprenticeshipKeyByApprenticeshipIdQueryHandler _sut;

    [SetUp]
    public void Setup()
    {

        _fixture = new Fixture();
        _approvalQueryRepository = new Mock<IApprovalQueryRepository>();
        _sut = new GetApprenticeshipKeyByApprenticeshipIdQueryHandler(_approvalQueryRepository.Object);
    }

    [Test]
    public async Task ThenApprenticeshipKeyIsReturned()
    {
        //Arrange
        var query = _fixture.Create<GetApprenticeshipKeyByApprenticeshipIdRequest>();
        var expectedResult = _fixture.Create<GetApprenticeshipKeyByApprenticeshipIdResponse>();

        _approvalQueryRepository
            .Setup(x => x.GetKeyByApprenticeshipId(query.ApprenticeshipId))
            .ReturnsAsync(expectedResult.ApprenticeshipKey);

        //Act
        var actualResult = await _sut.Handle(query);

        //Assert
        actualResult.Should().BeEquivalentTo(expectedResult);
    }
}