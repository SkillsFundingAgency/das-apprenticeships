using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Command.CancelPendingStartDateChange;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Apprenticeships.Command.UnitTests.CancelPendingStartDateChange;

[TestFixture]
public class WhenAPendingStartDateChangeIsCancelled
{
    private CancelPendingStartDateChangeCommandHandler _commandHandler = null!;
    private Mock<IApprenticeshipRepository> _apprenticeshipRepository = null!;
    private Fixture _fixture = null!;

    [SetUp]
    public void SetUp()
    {
        _apprenticeshipRepository = new Mock<IApprenticeshipRepository>();
        _commandHandler = new CancelPendingStartDateChangeCommandHandler(_apprenticeshipRepository.Object);

        _fixture = new Fixture();
        _fixture.Customize(new ApprenticeshipCustomization());
    }

    [Test]
    public async Task ThenStartDateChangeIsCancelled()
    {
        var command = _fixture.Create<CancelPendingStartDateChangeRequest>();
        var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
        apprenticeship.AddStartDateChange(
            _fixture.Create<DateTime>(), 
            _fixture.Create<string>(), 
            _fixture.Create<string>(), 
            _fixture.Create<DateTime>(), 
            _fixture.Create<string>(), 
            _fixture.Create<DateTime>(), 
            _fixture.Create<DateTime>(),
            ChangeRequestStatus.Created,
            ChangeInitiator.Provider);

        _apprenticeshipRepository.Setup(x => x.Get(command.ApprenticeshipKey)).ReturnsAsync(apprenticeship);

        await _commandHandler.Handle(command);

        _apprenticeshipRepository.Verify(x => x.Update(It.Is<ApprenticeshipDomainModel>(y => y.GetEntity().StartDateChanges.Count(z => z.RequestStatus == ChangeRequestStatus.Cancelled) == 1)));
    }
}