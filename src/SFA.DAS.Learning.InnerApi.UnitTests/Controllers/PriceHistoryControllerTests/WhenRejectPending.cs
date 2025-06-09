using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Learning.Command;
using SFA.DAS.Learning.Command.RejectPendingPriceChange;
using SFA.DAS.Learning.InnerApi.Controllers;
using SFA.DAS.Learning.InnerApi.Identity.Authorization;
using SFA.DAS.Learning.InnerApi.Requests;
using SFA.DAS.Learning.Queries;

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
            var httpContext = new DefaultHttpContext();
            _sut.ControllerContext = new ControllerContext();
            _sut.ControllerContext.HttpContext = httpContext;
        }

        [TestCase(UserType.Provider)]
        [TestCase(UserType.Employer)]
        public async Task ThenPendingPriceChangeIsRejected(UserType expectedRejector)
        {
            //  Arrange
            var apprenticeshipKey = _fixture.Create<Guid>();
            var request = _fixture.Create<PatchRejectPriceChangeRequest>();
            switch(expectedRejector)
            {
                case UserType.Provider:
                    _sut.ControllerContext.HttpContext.User = TestAuthorizationHelper.CreateClaimsPrincipleForProvider();
                    break;
                case UserType.Employer:
                    _sut.ControllerContext.HttpContext.User = TestAuthorizationHelper.CreateClaimsPrincipleForEmployer();
                    break;
            }

            //  Act
            var result = await _sut.RejectPendingPriceChange(apprenticeshipKey, request);

            //  Assert
            result.Should().BeOfType<OkObjectResult>();

            _commandDispatcher
                .Verify(x =>
                    x.Send(
                        It.Is<RejectPendingPriceChangeRequest>(r => r.ApprenticeshipKey == apprenticeshipKey && r.Reason == request.Reason), It.IsAny<CancellationToken>()), Times.Once);

            var rejectorResult = GetValueFromAnonymousType(result, "Rejector");
            rejectorResult.Should().Be(expectedRejector.ToString());

        }

        private string GetValueFromAnonymousType(IActionResult actionResult, string propertyName)
        {
            var anonymousObject = (OkObjectResult)actionResult;
            var data = anonymousObject.Value;
            return data!.GetType().GetProperty(propertyName)?.GetValue(data, null)?.ToString()!;
        }
    }
}