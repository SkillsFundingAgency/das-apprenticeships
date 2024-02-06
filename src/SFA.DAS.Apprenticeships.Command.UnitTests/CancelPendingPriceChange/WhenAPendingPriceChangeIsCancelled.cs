using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Command.CancelPendingPriceChange;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;

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
            var logger = new Mock<ILogger<CancelPendingPriceChangeCommandHandler>>();
            _apprenticeshipRepository = new Mock<IApprenticeshipRepository>();
            _commandHandler = new CancelPendingPriceChangeCommandHandler(_apprenticeshipRepository.Object, logger.Object);

            _fixture = new Fixture();
            _fixture.Customize(new ApprenticeshipCustomization());
        }

        [Test]
        public async Task ThenPriceHistoryIsCancelled()
        {
            var command = _fixture.Create<CancelPendingPriceChangeRequest>();
            var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
            apprenticeship.AddPriceHistory(_fixture.Create<decimal>(), _fixture.Create<decimal>(), _fixture.Create<decimal>(), _fixture.Create<DateTime>(), _fixture.Create<DateTime>(), PriceChangeRequestStatus.Created, _fixture.Create<string>(), _fixture.Create<string>());

            _apprenticeshipRepository.Setup(x => x.Get(command.ApprenticeshipKey)).ReturnsAsync(apprenticeship);
            
            await _commandHandler.Handle(command);
            
            _apprenticeshipRepository.Verify(x => x.Update(It.Is<ApprenticeshipDomainModel>(y => y.GetEntity().PriceHistories.Count() == 0)));
        }
    }
}
