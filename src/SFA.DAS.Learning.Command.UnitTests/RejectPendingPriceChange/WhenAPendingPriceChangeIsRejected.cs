using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Learning.Command.RejectPendingPriceChange;
using SFA.DAS.Learning.Domain.Apprenticeship;
using SFA.DAS.Learning.Domain.Repositories;
using SFA.DAS.Learning.Enums;
using SFA.DAS.Learning.TestHelpers.AutoFixture.Customizations;

namespace SFA.DAS.Learning.Command.UnitTests.RejectPendingPriceChange
{
    [TestFixture]
    public class WhenAPendingPriceChangeIsRejected
    {
        private RejectPendingPriceChangeCommandHandler _commandHandler = null!;
        private Mock<ILearningRepository> _apprenticeshipRepository = null!;
        private Fixture _fixture = null!;

        [SetUp]
        public void SetUp()
        {
            _apprenticeshipRepository = new Mock<ILearningRepository>();
            _commandHandler = new RejectPendingPriceChangeCommandHandler(_apprenticeshipRepository.Object);

            _fixture = new Fixture();
            _fixture.Customize(new ApprenticeshipCustomization());
        }

        [Test]
        public async Task ThenPriceHistoryIsCancelled()
        {
            var command = _fixture.Create<RejectPendingPriceChangeRequest>();
            var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
            apprenticeship.AddPriceHistory(_fixture.Create<decimal>(), _fixture.Create<decimal>(), _fixture.Create<decimal>(), _fixture.Create<DateTime>(), _fixture.Create<DateTime>(), ChangeRequestStatus.Created, _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<DateTime>(), _fixture.Create<DateTime>(), _fixture.Create<ChangeInitiator>());

            _apprenticeshipRepository.Setup(x => x.Get(command.LearningKey)).ReturnsAsync(apprenticeship);
            
            await _commandHandler.Handle(command);
            
            _apprenticeshipRepository.Verify(x => x.Update(It.Is<ApprenticeshipDomainModel>(y => y.GetEntity().PriceHistories.Count(z => z.PriceChangeRequestStatus == ChangeRequestStatus.Rejected) == 1)));
        }
    }
}
