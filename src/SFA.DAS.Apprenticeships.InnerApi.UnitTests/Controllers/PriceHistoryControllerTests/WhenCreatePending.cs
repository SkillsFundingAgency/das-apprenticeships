using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Apprenticeships.Command;
using SFA.DAS.Apprenticeships.Command.CreatePriceChange;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.InnerApi.Controllers;
using SFA.DAS.Apprenticeships.InnerApi.Requests;
using SFA.DAS.Apprenticeships.Queries;

namespace SFA.DAS.Apprenticeships.InnerApi.UnitTests.Controllers.PriceHistoryControllerTests;

public class WhenCreatePending
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
    public async Task ThenOkResultIsReturned()
    {
        var apprenticeshipKey = _fixture.Create<Guid>();
        var request = _fixture.Create<PostCreateApprenticeshipPriceChangeRequest>();
        var result = await _sut.CreateApprenticeshipPriceChange(apprenticeshipKey, request);

        result.Should().BeOfType<OkObjectResult>();

        _commandDispatcher.Verify(x => x.Send<CreatePriceChangeCommand, ChangeRequestStatus>(It.Is<CreatePriceChangeCommand>(r =>
            r.Initiator == request.Initiator &&
            r.ApprenticeshipKey == apprenticeshipKey &&
            r.UserId == request.UserId &&
            r.TrainingPrice == request.TrainingPrice &&
            r.AssessmentPrice == request.AssessmentPrice &&
            r.TotalPrice == request.TotalPrice &&
            r.Reason == request.Reason &&
            r.EffectiveFromDate == request.EffectiveFromDate
        ), It.IsAny<CancellationToken>()));
    }

    [Test]
    public async Task ThenBadRequestIsReturnedWhenInvalidRequest()
    {
        var apprenticeshipKey = _fixture.Create<Guid>();
        var request = _fixture.Create<PostCreateApprenticeshipPriceChangeRequest>();

        _commandDispatcher.Setup(x => x.Send<CreatePriceChangeCommand, ChangeRequestStatus>(It.Is<CreatePriceChangeCommand>(r =>
            r.Initiator == request.Initiator &&
            r.ApprenticeshipKey == apprenticeshipKey &&
            r.UserId == request.UserId &&
            r.TrainingPrice == request.TrainingPrice &&
            r.AssessmentPrice == request.AssessmentPrice &&
            r.TotalPrice == request.TotalPrice &&
            r.Reason == request.Reason &&
            r.EffectiveFromDate == request.EffectiveFromDate
        ), It.IsAny<CancellationToken>())).Throws<ArgumentException>();

        var result = await _sut.CreateApprenticeshipPriceChange(apprenticeshipKey, request);

        result.Should().BeOfType<BadRequestResult>();
    }
}