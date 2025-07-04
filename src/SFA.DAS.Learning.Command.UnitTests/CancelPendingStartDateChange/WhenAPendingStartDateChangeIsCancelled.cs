using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Learning.Command.CancelPendingStartDateChange;
using SFA.DAS.Learning.Domain.Apprenticeship;
using SFA.DAS.Learning.Domain.Repositories;
using SFA.DAS.Learning.Enums;
using SFA.DAS.Learning.TestHelpers.AutoFixture.Customizations;

namespace SFA.DAS.Learning.Command.UnitTests.CancelPendingStartDateChange;

[TestFixture]
public class WhenAPendingStartDateChangeIsCancelled
{
    private CancelPendingStartDateChangeCommandHandler _commandHandler = null!;
    private Mock<ILearningRepository> _apprenticeshipRepository = null!;
    private Fixture _fixture = null!;

    [SetUp]
    public void SetUp()
    {
        _apprenticeshipRepository = new Mock<ILearningRepository>();
        _commandHandler = new CancelPendingStartDateChangeCommandHandler(_apprenticeshipRepository.Object);

        _fixture = new Fixture();
        _fixture.Customize(new ApprenticeshipCustomization());
    }

    [Test]
    public async Task ThenStartDateChangeIsCancelled()
    {
        var command = _fixture.Create<CancelPendingStartDateChangeRequest>();
        var apprenticeship = _fixture.Create<LearningDomainModel>();
        apprenticeship.AddStartDateChange(
            _fixture.Create<DateTime>(),    // ActualStartDate
            _fixture.Create<DateTime>(),    // PlannedEndDate
            _fixture.Create<string>(),      // Reason
            _fixture.Create<string>(),      // ProviderApprovedBy 
            _fixture.Create<DateTime>(),    // ProviderApprovedByDate 
            _fixture.Create<string>(),      //EmployerApprovedBy 
            _fixture.Create<DateTime>(),    //EmployerApprovedByDate
            _fixture.Create<DateTime>(),    //CreatedDate
            ChangeRequestStatus.Created,    // RequestStatus
            ChangeInitiator.Provider);      // ChangeInitiator

        _apprenticeshipRepository.Setup(x => x.Get(command.ApprenticeshipKey)).ReturnsAsync(apprenticeship);

        await _commandHandler.Handle(command);

        _apprenticeshipRepository.Verify(x => x.Update(It.Is<LearningDomainModel>(y => y.GetEntity().StartDateChanges.Count(z => z.RequestStatus == ChangeRequestStatus.Cancelled) == 1)));
    }
}