using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;
using SFA.DAS.Learning.Command.CancelPendingPriceChange;
using SFA.DAS.Learning.Domain.Apprenticeship;
using SFA.DAS.Learning.Domain.Repositories;
using SFA.DAS.Learning.Enums;

namespace SFA.DAS.Apprenticeships.Command.UnitTests.CancelPendingPriceChange
{
    [TestFixture]
    public class WhenAPendingPriceChangeIsCancelled
    {
        private CancelPendingPriceChangeCommandHandler _commandHandler = null!;
        private Mock<IApprenticeshipRepository> _apprenticeshipRepository = null!;
        private Fixture _fixture = null!;

        [SetUp]
        public void SetUp()
        {
            _apprenticeshipRepository = new Mock<IApprenticeshipRepository>();
            _commandHandler = new CancelPendingPriceChangeCommandHandler(_apprenticeshipRepository.Object);

            _fixture = new Fixture();
            _fixture.Customize(new ApprenticeshipCustomization());
        }

        [Test]
        public async Task ThenPriceHistoryIsCancelled()
        {
            var command = _fixture.Create<CancelPendingPriceChangeRequest>();
            var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
            apprenticeship.AddPriceHistory(_fixture.Create<decimal>(), _fixture.Create<decimal>(), _fixture.Create<decimal>(), _fixture.Create<DateTime>(), _fixture.Create<DateTime>(), ChangeRequestStatus.Created, _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<DateTime>(), _fixture.Create<DateTime>(), _fixture.Create<ChangeInitiator>());

            _apprenticeshipRepository.Setup(x => x.Get(command.ApprenticeshipKey)).ReturnsAsync(apprenticeship);
            
            await _commandHandler.Handle(command);
            
            _apprenticeshipRepository.Verify(x => x.Update(It.Is<ApprenticeshipDomainModel>(y => y.GetEntity().PriceHistories.Count(z => z.PriceChangeRequestStatus == ChangeRequestStatus.Cancelled) == 1)));
        }
    }
}
