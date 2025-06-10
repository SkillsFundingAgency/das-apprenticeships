using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Learning.Command;
using SFA.DAS.Learning.InnerApi.Controllers;
using SFA.DAS.Learning.Queries;
using SFA.DAS.Learning.Queries.GetPendingPriceChange;

namespace SFA.DAS.Learning.InnerApi.UnitTests.Controllers.PriceHistoryControllerTests
{
    public class WhenGetPending
    {
        private Fixture _fixture;
        private Mock<IQueryDispatcher> _queryDispatcher;
        private PriceHistoryController _sut;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _queryDispatcher = new Mock<IQueryDispatcher>();
            _sut = new PriceHistoryController(_queryDispatcher.Object, Mock.Of<ICommandDispatcher>(), Mock.Of<ILogger<PriceHistoryController>>());
        }

        [Test]
        public async Task ThenPendingPriceChangeIsReturned()
        {
            var apprenticeshipKey = _fixture.Create<Guid>();
            var expectedResult = _fixture.Create<GetPendingPriceChangeResponse>();

            _queryDispatcher
                .Setup(x => x.Send<GetPendingPriceChangeRequest, GetPendingPriceChangeResponse>(It.Is<GetPendingPriceChangeRequest>(r => r.ApprenticeshipKey == apprenticeshipKey)))
                .ReturnsAsync(expectedResult);

            var result = await _sut.GetPendingPriceChange(apprenticeshipKey);

            result.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)result;
            okResult.Value.Should().Be(expectedResult);
        }
    }
}