using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Command;
using SFA.DAS.Apprenticeships.Command.ChangePrice;
using SFA.DAS.Approvals.EventHandlers.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Apprenticeships.Functions.UnitTests;

public class WhenHandlePriceChangeApprovedTests
{
    private HandlePriceChangeApproved _sut;
    private Mock<ICommandDispatcher> _commandDispatcher;
    private Fixture _fixture;

    [SetUp]

    public void Setup()
    {
        _fixture = new Fixture();
        _commandDispatcher = new Mock<ICommandDispatcher>();
        _sut = new HandlePriceChangeApproved(_commandDispatcher.Object);
    }

    [Test]
    public async Task ThenApprovalIsAdded()
    {
        var approvalsEvent = _fixture.Create<PriceChangeApprovedByEmployer>();
        await _sut.HandleCommand(approvalsEvent);

        _commandDispatcher.Verify(x =>
            x.Send(It.Is<ChangePriceCommand>(c =>
                    c.ApprovalsApprenticeshipId == approvalsEvent.ApprenticeshipId &&
                    c.ApprovedDate == approvalsEvent.ApprovedDate &&
                    c.AssessmentPrice == approvalsEvent.AssessmentPrice &&
                    c.EffectiveFrom == approvalsEvent.EffectiveFrom &&
                    c.EmployerAccountId == approvalsEvent.EmployerAccountId &&
                    c.TrainingPrice == approvalsEvent.TrainingPrice &&
                    c.AssessmentPrice == approvalsEvent.AssessmentPrice
                ),
                It.IsAny<CancellationToken>()));
    }
}