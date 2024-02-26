using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Apprenticeships.Command;
using SFA.DAS.Apprenticeships.Command.CancelPendingPriceChange;
using SFA.DAS.Apprenticeships.InnerApi.Controllers;
using SFA.DAS.Apprenticeships.Queries;

namespace SFA.DAS.Apprenticeships.InnerApi.UnitTests.Controllers.PriceHistoryControllerTests
{
    public class WhenDeletePending
    {
        private Fixture _fixture;
        private Mock<ICommandDispatcher> _commandDispatcher;
        private PriceHistoryController _sut;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _commandDispatcher = new Mock<ICommandDispatcher>();
            _sut = new PriceHistoryController(Mock.Of<IQueryDispatcher>(), _commandDispatcher.Object);
        }

        [Test]
        public async Task ThenPendingPriceChangeIsCancelled()
        {
            var apprenticeshipKey = _fixture.Create<Guid>();
            
            var result = await _sut.CancelPendingPriceChange(apprenticeshipKey);

            result.Should().BeOfType<OkResult>();
            _commandDispatcher
                .Verify(x =>
                    x.Send(
                        It.Is<CancelPendingPriceChangeRequest>(r => r.ApprenticeshipKey == apprenticeshipKey), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}