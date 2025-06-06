using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Apprenticeships.Command;
using SFA.DAS.Apprenticeships.Command.ApprovePriceChange;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.InnerApi.Controllers;
using SFA.DAS.Apprenticeships.InnerApi.Requests;
using SFA.DAS.Apprenticeships.Queries;

namespace SFA.DAS.Apprenticeships.InnerApi.UnitTests.Controllers.PriceHistoryControllerTests
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
                        It.Is<ApprovePriceChangeCommand>(r => r.ApprenticeshipKey == apprenticeshipKey && r.UserId == request.UserId), 
                        It.IsAny<CancellationToken>()), Times.Once);
            result.Should().BeOfType<OkObjectResult>();
        }
    }
}