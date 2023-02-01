using AutoFixture;
using FluentAssertions;
using Moq;
using SFA.DAS.Apprenticeships.DataTransferObjects;
using SFA.DAS.Apprenticeships.Domain.Repositories;

namespace SFA.DAS.Apprenticeships.Queries.UnitTests
{
    public class WhenGetApprenticeships
    {
        private Fixture _fixture;
        private Mock<IApprenticeshipQueryRepository> _apprenticeshipQueryRepository;
        private GetApprenticeshipsQueryHandler _sut;

        [SetUp]
        public void Setup()
        {

            _fixture = new Fixture();
            _apprenticeshipQueryRepository = new Mock<IApprenticeshipQueryRepository>();
            _sut = new GetApprenticeshipsQueryHandler(_apprenticeshipQueryRepository.Object);
        }

        [Test]
        public async Task ThenApprenticeshipsAreReturned()
        {
            //Arrange
            var query = _fixture.Create<GetApprenticeshipsRequest>();
            var expectedResult = _fixture.Create<IEnumerable<Apprenticeship>>();

            _apprenticeshipQueryRepository
                .Setup(x => x.GetAll(query.Ukprn, query.FundingPlatform))
                .ReturnsAsync(expectedResult);

            //Act
            var actualResult = await _sut.Handle(query);

            //Assert
            actualResult.Apprenticeships.Should().BeEquivalentTo(expectedResult);
        }
    }
}