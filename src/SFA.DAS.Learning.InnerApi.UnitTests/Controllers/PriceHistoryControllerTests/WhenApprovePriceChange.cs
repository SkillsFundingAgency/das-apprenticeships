using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Learning.Command;
using SFA.DAS.Learning.Command.ApprovePriceChange;
using SFA.DAS.Learning.Enums;
using SFA.DAS.Learning.InnerApi.Controllers;
using SFA.DAS.Learning.InnerApi.Requests;
using SFA.DAS.Learning.Queries;

namespace SFA.DAS.Learning.InnerApi.UnitTests.Controllers.PriceHistoryControllerTests
{
    public class WhenApprovePriceChange
    {
        private Fixture _fixture;
        private Mock<IQueryDispatcher> _queryDispatcher;
        private Mock<ICommandDispatcher> _commandDispatcher;
        private PriceHistoryController _sut;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _queryDispatcher = new Mock<IQueryDispatcher>();
            _commandDispatcher = new Mock<ICommandDispatcher>();
            var logger = Mock.Of<ILogger<PriceHistoryController>>();
            _sut = new PriceHistoryController(_queryDispatcher.Object, _commandDispatcher.Object, logger);
        }

        [Test]
        public async Task ThenPendingPriceChangeIsReturned()
        {
            var apprenticeshipKey = _fixture.Create<Guid>();
            var request = _fixture.Create<ApprovePriceChangeRequest>();

            var result = await _sut.ApprovePriceChange(apprenticeshipKey, request);

            _commandDispatcher.Verify(x => x.Send<ApprovePriceChangeCommand, ApprovedBy> (
                        It.Is<ApprovePriceChangeCommand>(r => r.LearningKey == apprenticeshipKey && r.UserId == request.UserId), 
                        It.IsAny<CancellationToken>()), Times.Once);
            result.Should().BeOfType<OkObjectResult>();
        }
    }
}