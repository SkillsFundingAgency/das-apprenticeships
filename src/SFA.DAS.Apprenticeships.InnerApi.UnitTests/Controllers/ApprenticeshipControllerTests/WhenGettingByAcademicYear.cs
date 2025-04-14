using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Apprenticeships.Command;
using SFA.DAS.Apprenticeships.Domain;
using SFA.DAS.Apprenticeships.InnerApi.Controllers;
using SFA.DAS.Apprenticeships.Queries;
using SFA.DAS.Apprenticeships.Queries.GetApprenticeshipsByAcademicYear;

namespace SFA.DAS.Apprenticeships.InnerApi.UnitTests.Controllers.ApprenticeshipControllerTests;

public class WhenGettingByAcademicYear
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
        _sut = new ApprenticeshipController(_queryDispatcher.Object, _commandDispatcher.Object, _mockLogger.Object);
    }

    [Test]
    public async Task ThenApprenticeshipPriceIsReturned()
    {
        var ukprn = _fixture.Create<long>();
        const int academicYear = 2526;
        var expectedResult = _fixture.Create<GetApprenticeshipsByAcademicYearResponse>();

        _queryDispatcher
            .Setup(x => x.Send<GetApprenticeshipsByAcademicYearRequest, GetApprenticeshipsByAcademicYearResponse>(
                It.Is<GetApprenticeshipsByAcademicYearRequest>(r => r.UkPrn == ukprn && r.AcademicYear == academicYear && r.Page == 1))
            ).ReturnsAsync(expectedResult);

        var result = await _sut.GetByAcademicYear(ukprn, academicYear, 1);

        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.Value.Should().Be(expectedResult);
    }
}