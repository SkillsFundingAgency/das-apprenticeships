using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Command.AddApproval;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Factories;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;

namespace SFA.DAS.Apprenticeships.Command.UnitTests.AddApproval
{
    [TestFixture]
    public class WhenAnApprovalIsAdded
    {
        private AddApprovalCommandHandler _commandHandler;
        private Mock<IApprenticeshipFactory> _apprenticeshipFactory;
        private Mock<IApprenticeshipRepository> _apprenticeshipRepository;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _apprenticeshipFactory = new Mock<IApprenticeshipFactory>();
            _apprenticeshipRepository = new Mock<IApprenticeshipRepository>();
            _commandHandler = new AddApprovalCommandHandler(_apprenticeshipFactory.Object, _apprenticeshipRepository.Object);

            _fixture = new Fixture();
            _fixture.Customize(new ApprenticeshipCustomization());
        }

        [Test]
        public async Task ThenTheApprovalIsCreated()
        {
            var command = _fixture.Create<AddApprovalCommand>();
            var apprenticeship = _fixture.Create<Apprenticeship>();

            _apprenticeshipFactory.Setup(x => x.CreateNew(command.Uln, command.TrainingCode, command.DateOfBirth)).Returns(apprenticeship);

            await _commandHandler.Handle(command);

            _apprenticeshipRepository.Verify(x => x.Add(It.Is<Apprenticeship>(y => y.GetModel().Approvals.Count == 1)));
        }
    }
}
