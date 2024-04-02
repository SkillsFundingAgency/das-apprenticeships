﻿using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Apprenticeships.InnerApi.Controllers;
using SFA.DAS.Apprenticeships.Queries;
using SFA.DAS.Apprenticeships.Queries.GetApprenticeshipStartDate;

namespace SFA.DAS.Apprenticeships.InnerApi.UnitTests.Controllers.ApprenticeshipControllerTests;

public class WhenGetApprenticeshipStartDate
{
    private Fixture _fixture;
    private Mock<IQueryDispatcher> _queryDispatcher;
    private ApprenticeshipController _sut;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _queryDispatcher = new Mock<IQueryDispatcher>();
        _sut = new ApprenticeshipController(_queryDispatcher.Object);
    }

    [Test]
    public async Task ThenApprenticeshipStartDateIsReturned()
    {
        var apprenticeshipKey = _fixture.Create<Guid>();
        var expectedResult = _fixture.Create<GetApprenticeshipStartDateResponse>();

        _queryDispatcher
            .Setup(x => x.Send<GetApprenticeshipStartDateRequest, GetApprenticeshipStartDateResponse>(It.Is<GetApprenticeshipStartDateRequest>(r => r.ApprenticeshipKey == apprenticeshipKey)))
            .ReturnsAsync(expectedResult);

        var result = await _sut.GetStartDate(apprenticeshipKey);

        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.Value.Should().Be(expectedResult);
    }
    
    [Test]
    public async Task ThenNotFoundIsReturnedWhenNoRecordExists()
    {
        var apprenticeshipKey = _fixture.Create<Guid>(); ;

        _queryDispatcher
            .Setup(x => x.Send<GetApprenticeshipStartDateRequest, GetApprenticeshipStartDateResponse>(It.Is<GetApprenticeshipStartDateRequest>(r => r.ApprenticeshipKey == apprenticeshipKey)))
            .ReturnsAsync((GetApprenticeshipStartDateResponse)null!);

        var result = await _sut.GetStartDate(apprenticeshipKey);

        result.Should().BeOfType<NotFoundResult>();
    }
}