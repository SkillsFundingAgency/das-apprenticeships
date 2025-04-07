using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Apprenticeships.Command;
using SFA.DAS.Apprenticeships.Domain;
using SFA.DAS.Apprenticeships.InnerApi.Controllers;
using SFA.DAS.Apprenticeships.InnerApi.Services;
using SFA.DAS.Apprenticeships.Queries;
using SFA.DAS.Apprenticeships.Queries.GetApprenticeshipsByDates;

namespace SFA.DAS.Apprenticeships.InnerApi.UnitTests.Controllers.ApprenticeshipControllerTests;

public class WhenGettingByDates
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

    [Test]
    public async Task ThenApprenticeshipPriceIsReturned()
    {
        var ukprn = _fixture.Create<long>();
        var dates = new DateRange(DateTime.Today, DateTime.Today.AddDays(100));
        var expectedResult = _fixture.Create<GetApprenticeshipsByDatesResponse>();

        _queryDispatcher
            .Setup(x => x.Send<GetApprenticeshipsByDatesRequest, GetApprenticeshipsByDatesResponse>(
                It.Is<GetApprenticeshipsByDatesRequest>(r => r.UkPrn == ukprn && r.Dates == dates && r.Page == 1))
            ).ReturnsAsync(expectedResult);

        var result = await _sut.GetByDates(ukprn, dates.Start.ToShortDateString(), dates.End.ToShortDateString(), 1);

        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.Value.Should().Be(expectedResult);
    }

    [Test]
    public async Task ThenBadRequestIsReturnedWhenInvalidStartDateIsSent()
    {
        var ukprn = _fixture.Create<long>();
        const string startDate = "ABCD";

        var result = await _sut.GetByDates(ukprn, startDate, DateTime.Today.ToShortDateString(), 1);

        result.Should().BeOfType<BadRequestResult>();
    }
    
    [Test]
    public async Task ThenBadRequestIsReturnedWhenInvalidEndDateIsSent()
    {
        var ukprn = _fixture.Create<long>();
        const string endDate = "ABCD";

        var result = await _sut.GetByDates(ukprn, DateTime.Today.ToShortDateString(), endDate, 1);

        result.Should().BeOfType<BadRequestResult>();
    }
}