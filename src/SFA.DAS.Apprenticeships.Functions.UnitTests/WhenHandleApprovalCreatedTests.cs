using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Command;
using SFA.DAS.Apprenticeships.Command.AddApproval;
using SFA.DAS.Approvals.EventHandlers.Messages;
using SFA.DAS.Approvals.EventHandlers.Messages;

namespace SFA.DAS.Apprenticeships.Functions.UnitTests
{
    public class WhenHandleApprovalCreatedTests
    {
        private Mock<ICommandDispatcher> _commandDispatcher;
        private HandleApprovalCreated _handler;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _commandDispatcher = new Mock<ICommandDispatcher>();
            _handler = new HandleApprovalCreated(_commandDispatcher.Object);
        }

        [Test]
        public async Task ThenApprovalIsAdded()
        {
            var command = _fixture.Create<ApprovalCreatedEvent>();
            command.FundingType = FundingType.Transfer;
            await _handler.HandleCommand(command);

            _commandDispatcher.Verify(x =>
                x.Send<AddApprovalCommand>(It.Is<AddApprovalCommand>(c =>
                        c.TrainingCode == command.TrainingCode &&
                        c.ActualStartDate == command.ActualStartDate &&
                        c.AgreedPrice == command.AgreedPrice &&
                        c.ApprovalsApprenticeshipId == command.ApprovalsApprenticeshipId &&
                        c.EmployerAccountId == command.EmployerAccountId &&
                        c.FundingEmployerAccountId == command.FundingEmployerAccountId &&
                        c.FundingType == Enums.FundingType.Transfer &&
                        c.LegalEntityName == command.LegalEntityName &&
                        c.PlannedEndDate == command.PlannedEndDate &&
                        c.UKPRN == command.UKPRN &&
                        c.Uln == command.Uln //&&
                       // c.DateOfBirth == command.DateOfBirth TODO
                    ),
                    It.IsAny<CancellationToken>()));
        }
    }
}