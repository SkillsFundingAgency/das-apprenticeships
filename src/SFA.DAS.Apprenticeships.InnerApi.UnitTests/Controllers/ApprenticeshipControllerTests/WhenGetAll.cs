using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Apprenticeships.Command;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.InnerApi.Controllers;
using SFA.DAS.Apprenticeships.InnerApi.Services;
using SFA.DAS.Apprenticeships.Queries;
using SFA.DAS.Apprenticeships.Queries.GetApprenticeships;

namespace SFA.DAS.Apprenticeships.InnerApi.UnitTests.Controllers.ApprenticeshipControllerTests
{
    public class WhenGetAll
    {
        private Fixture _fixture;
        private Mock<IQueryDispatcher> _queryDispatcher;
        private Mock<ICommandDispatcher> _commandDispatcher;
        private Mock<ILogger<ApprenticeshipController>> _mockLogger;
        private ApprenticeshipController _sut;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _queryDispatcher = new Mock<IQueryDispatcher>();
            _commandDispatcher = new Mock<ICommandDispatcher>();
            _mockLogger = new Mock<ILogger<ApprenticeshipController>>();
            _sut = new ApprenticeshipController(_queryDispatcher.Object, _commandDispatcher.Object, _mockLogger.Object, Mock.Of<IPagedLinkHeaderService>());
        }

        [TestCase(null)]
        [TestCase(FundingPlatform.SLD)]
        [TestCase(FundingPlatform.DAS)]
        public async Task ThenApprenticeshipsAreReturned(FundingPlatform? fundingPlatform)
        {
            var ukprn = _fixture.Create<long>();
            var expectedResult = _fixture.Create<GetApprenticeshipsResponse>();

            _queryDispatcher
                .Setup(x => x.Send<GetApprenticeshipsRequest, GetApprenticeshipsResponse>(It.Is<GetApprenticeshipsRequest>(r => r.Ukprn == ukprn && r.FundingPlatform == fundingPlatform)))
                .ReturnsAsync(expectedResult);

            var result = await _sut.GetAll(ukprn, fundingPlatform);

            result.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)result;
            okResult.Value.Should().Be(expectedResult.Apprenticeships);
        }
    }
}