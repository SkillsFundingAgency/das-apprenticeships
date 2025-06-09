using AutoFixture;
using FluentAssertions;
using Moq;
using SFA.DAS.Learning.DataTransferObjects;
using SFA.DAS.Learning.Domain.Repositories;
using SFA.DAS.Learning.Queries.GetPendingStartDateChange;

namespace SFA.DAS.Learning.Queries.UnitTests
{
    public class WhenGetPendingStartDateChange
    {
        private Fixture _fixture;
        private Mock<IApprenticeshipQueryRepository> _apprenticeshipQueryRepository;
        private GetPendingStartDateChangeQueryHandler _sut;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _apprenticeshipQueryRepository = new Mock<IApprenticeshipQueryRepository>();
            _sut = new GetPendingStartDateChangeQueryHandler(_apprenticeshipQueryRepository.Object);
        }

        [Test]
        public async Task ThenWhenPendingStartDateChangeExists_PendingStartDateChangeIsReturned()
        {
            // Arrange
            var query = _fixture.Create<GetPendingStartDateChangeRequest>();
            var expectedResult = _fixture.Create<PendingStartDateChange>();

            _apprenticeshipQueryRepository
                .Setup(x => x.GetPendingStartDateChange(query.ApprenticeshipKey))
                .ReturnsAsync(expectedResult);

            // Act
            var actualResult = await _sut.Handle(query);

            // Assert
            actualResult.HasPendingStartDateChange.Should().BeTrue();
            actualResult.PendingStartDateChange.Should().Be(expectedResult);
        }

        [Test]
        public async Task ThenWhenPendingStartDateChangeDoesNotExist_PendingStartDateChangeIsNotReturned()
        {
            // Arrange
            var query = _fixture.Create<GetPendingStartDateChangeRequest>();

            _apprenticeshipQueryRepository
                .Setup(x => x.GetPendingStartDateChange(query.ApprenticeshipKey))
                .ReturnsAsync((PendingStartDateChange)null!);

            // Act
            var actualResult = await _sut.Handle(query);

            // Assert
            actualResult.HasPendingStartDateChange.Should().BeFalse();
            actualResult.PendingStartDateChange.Should().BeNull();
        }
    }
}
