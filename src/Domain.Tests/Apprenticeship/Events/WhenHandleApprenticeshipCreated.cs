using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Events;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Domain.UnitTests.Helpers;
using SFA.DAS.Apprenticeships.TestHelpers;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;
using SFA.DAS.Apprenticeships.Types;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Apprenticeship.Events;

public class WhenHandleApprenticeshipCreated
{
    private Mock<IApprenticeshipRepository> _apprenticeshipRepository;
    private Mock<IMessageSession> _messageSession;
    private ApprenticeshipCreatedHandler _handler;
    private Fixture _fixture;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _fixture.Customize(new ApprenticeshipCustomization());
        _apprenticeshipRepository = new Mock<IApprenticeshipRepository>();
        _messageSession = new Mock<IMessageSession>();
        _handler = new ApprenticeshipCreatedHandler(_apprenticeshipRepository.Object, _messageSession.Object);
    }

    [Test]
    public async Task ThenApprenticeshipCreatedEventIsPublished()
    {
        var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
        ApprenticeshipDomainModelTestHelper.AddEpisode(apprenticeship);
            
        var command = _fixture.Create<ApprenticeshipCreated>();

        _apprenticeshipRepository.Setup(x => x.Get(command.ApprenticeshipKey)).ReturnsAsync(apprenticeship);

        await _handler.Handle(command);

        _messageSession.Verify(x => x.Publish(It.Is<ApprenticeshipCreatedEvent>(e =>
            DoApprenticeshipDetailsMatchDomainModel(e, apprenticeship) 
            && ApprenticeshipDomainModelTestHelper.DoEpisodeDetailsMatchDomainModel(e, apprenticeship)), It.IsAny<PublishOptions>()));
    }

    private static bool DoApprenticeshipDetailsMatchDomainModel(ApprenticeshipCreatedEvent e, ApprenticeshipDomainModel apprenticeship)
    {
        return
            e.ApprenticeshipKey == apprenticeship.Key &&
            e.ApprovalsApprenticeshipId == apprenticeship.ApprovalsApprenticeshipId &&
            e.Uln == apprenticeship.Uln &&
            e.FirstName == apprenticeship.FirstName &&
            e.LastName == apprenticeship.LastName &&
            e.DateOfBirth == apprenticeship.DateOfBirth;
    }
}