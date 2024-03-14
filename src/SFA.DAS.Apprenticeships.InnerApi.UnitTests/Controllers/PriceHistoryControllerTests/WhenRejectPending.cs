using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Apprenticeships.Command;
using SFA.DAS.Apprenticeships.Command.RejectPendingPriceChange;
using SFA.DAS.Apprenticeships.InnerApi.Controllers;
using SFA.DAS.Apprenticeships.InnerApi.Requests;
using SFA.DAS.Apprenticeships.Queries;

namespace SFA.DAS.Apprenticeships.InnerApi.UnitTests.Controllers.PriceHistoryControllerTests
{
    public class WhenRejectPending
    {
        private Fixture _fixture;
        private Mock<ICommandDispatcher> _commandDispatcher;
        private PriceHistoryController _sut;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _commandDispatcher = new Mock<ICommandDispatcher>();
            _sut = new PriceHistoryController(Mock.Of<IQueryDispatcher>(), _commandDispatcher.Object, Mock.Of<ILogger<PriceHistoryController>>());
        }

        [Test]
        public async Task ThenPendingPriceChangeIsRejected()
        {
            var apprenticeshipKey = _fixture.Create<Guid>();
            var request = _fixture.Create<PatchRejectPriceChangeRequest>();
            
            var result = await _sut.RejectPendingPriceChange(apprenticeshipKey, request);

            result.Should().BeOfType<OkResult>();
            _commandDispatcher
                .Verify(x =>
                    x.Send(
                        It.Is<RejectPendingPriceChangeRequest>(r => r.ApprenticeshipKey == apprenticeshipKey && r.Reason == request.Reason), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}