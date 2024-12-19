﻿using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Command.WithdrawApprenticeship;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Infrastructure.ApprenticeshipsOuterApiClient;
using SFA.DAS.Apprenticeships.Infrastructure.ApprenticeshipsOuterApiClient.Calendar;
using SFA.DAS.Apprenticeships.Infrastructure.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Apprenticeships.Command.UnitTests.WithdrawApprenticeship;

public class WhenHandleWithdrawCommand
{
    private Fixture _fixture;
    private Mock<IApprenticeshipRepository> _apprenticeshipRepository;
    private Mock<IApprenticeshipsOuterApiClient> _apprenticeshipsOuterApiClient;
    private Mock<ISystemClockService> _systemClockService;
    private Mock<IValidator<WithdrawApprenticeshipCommand>> _validator;
    private Mock<ILogger<WithdrawApprenticeshipCommandHandler>> _logger;

    private const long ValidUkprn = 1000000;

    public WhenHandleWithdrawCommand()
    {
        _fixture = new Fixture();
        _apprenticeshipRepository = new Mock<IApprenticeshipRepository>();
        _apprenticeshipsOuterApiClient = MockOuterApiAcademicYearEnd(2025, 7, 22);
        _validator = new Mock<IValidator<WithdrawApprenticeshipCommand>>();
        _systemClockService = MockSystemClock(2024, 12, 17);
        _logger = new Mock<ILogger<WithdrawApprenticeshipCommandHandler>>();
    }

    [Test]
    public async Task WhenInvalidRequestThenReturnsValidationError()
    {
        // Arrange
        ResetMockRepository();
        string message = "TestMessage";
        _validator.Setup(x => x.IsValid(It.IsAny<WithdrawApprenticeshipCommand>(), out message, It.IsAny<object?[]>()))
            .Returns(false);
        var sut = new WithdrawApprenticeshipCommandHandler(
            _apprenticeshipRepository.Object,
            _apprenticeshipsOuterApiClient.Object,
            _systemClockService.Object,
            _validator.Object,
            _logger.Object);

        var command = _fixture.Create<WithdrawApprenticeshipCommand>();

        // Act
        var result = await sut.Handle(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("TestMessage");
    }

    [Test]
    public async Task WhenValidRequestWithdrawCompleted()
    {
        // Arrange
        ResetMockRepository();
        string message = "";
        _validator.Setup(x => x.IsValid(It.IsAny<WithdrawApprenticeshipCommand>(), out message, It.IsAny<object?[]>()))
            .Returns(true);

        var sut = new WithdrawApprenticeshipCommandHandler(
            _apprenticeshipRepository.Object,
            _apprenticeshipsOuterApiClient.Object,
            _systemClockService.Object,
            _validator.Object,
            _logger.Object);

        var command = _fixture.Create<WithdrawApprenticeshipCommand>();

        // Act
        var result = await sut.Handle(command);

        // Assert
        _apprenticeshipRepository.Verify(x => x.Update(
            It.Is<ApprenticeshipDomainModel>(y =>
                y.GetEntity().WithdrawalRequests
                    .Count(z => z.Reason == command.Reason
                                && z.LastDayOfLearning == command.LastDayOfLearning) == 1
                && y.LatestEpisode.LearningStatus == LearnerStatus.Withdrawn)));
    }

    private Mock<IApprenticeshipRepository> ResetMockRepository()
    {
        var apprenticeship = ApprenticeshipDomainModelTestHelper.CreateBasicTestModel();

        ApprenticeshipDomainModelTestHelper.AddEpisode(apprenticeship, ukprn: ValidUkprn);

        _apprenticeshipRepository.Setup(x => x.GetByUln(It.IsAny<string>())).ReturnsAsync(apprenticeship);

        return _apprenticeshipRepository;
    }

    private static Mock<ISystemClockService> MockSystemClock(int year, byte month, byte date)
    {
        var systemClockService = new Mock<ISystemClockService>();
        systemClockService.Setup(x => x.UtcNow).Returns(new DateTime(year, month, date));
        return systemClockService;
    }

    private static Mock<IApprenticeshipsOuterApiClient> MockOuterApiAcademicYearEnd(int year, byte month, byte date)
    {
        var apprenticeshipsOuterApiClient = new Mock<IApprenticeshipsOuterApiClient>();
        apprenticeshipsOuterApiClient.Setup(x => x.GetAcademicYear(It.IsAny<DateTime>())).ReturnsAsync(new GetAcademicYearsResponse
        {
            EndDate = new DateTime(year, month, date)
        });
        return apprenticeshipsOuterApiClient;
    }

}