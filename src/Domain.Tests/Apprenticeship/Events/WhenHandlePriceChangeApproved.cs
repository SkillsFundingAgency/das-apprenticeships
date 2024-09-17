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
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;
using SFA.DAS.Apprenticeships.Types;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Apprenticeship.Events;

public class WhenHandlePriceChangeApproved
{
    private Mock<IApprenticeshipRepository> _apprenticeshipRepository;
    private Mock<IMessageSession> _messageSession;
    private PriceChangeApprovedHandler _handler;
    private Fixture _fixture;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _fixture.Customize(new ApprenticeshipCustomization());
        _apprenticeshipRepository = new Mock<IApprenticeshipRepository>();
        _messageSession = new Mock<IMessageSession>();
        _handler = new PriceChangeApprovedHandler(_apprenticeshipRepository.Object, _messageSession.Object);
    }

    [Test]
    public async Task ByEmployerThenPriceChangeApprovedEventIsPublished()
    {
        var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
        ApprenticeshipDomainModelTestHelper.AddEpisode(apprenticeship);
        ApprenticeshipDomainModelTestHelper.AddPendingPriceChangeProviderInitiated(apprenticeship, effectiveFromDate:apprenticeship.LatestPrice.StartDate.AddDays(_fixture.Create<int>()));
        apprenticeship.ApprovePriceChange("Bob", null, null);
        var priceChange = apprenticeship.PriceHistories.First(x => x.PriceChangeRequestStatus == ChangeRequestStatus.Approved);
        var domainEvent = new PriceChangeApproved(apprenticeship.Key, priceChange.Key, ApprovedBy.Employer);

        _apprenticeshipRepository.Setup(x => x.Get(domainEvent.ApprenticeshipKey)).ReturnsAsync(apprenticeship);

        await _handler.Handle(domainEvent);

        _messageSession.Verify(x => x.Publish(It.Is<ApprenticeshipPriceChangedEvent>(e =>
            DoApprenticeshipDetailsMatchDomainModel(e, apprenticeship) &&
            SendsApproverAndInittorDetails(e, priceChange) &&
            IsMarkedApprovedByEmployer(e, priceChange) &&
            e.EffectiveFromDate == priceChange.EffectiveFromDate &&
            ApprenticeshipDomainModelTestHelper.DoEpisodeDetailsMatchDomainModel(e, apprenticeship)), It.IsAny<PublishOptions>()));
    }

    [Test]
    public async Task ByProviderThenPriceChangeApprovedEventIsPublished()
    {
        var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
        ApprenticeshipDomainModelTestHelper.AddEpisode(apprenticeship);
        var trainingPrice = _fixture.Create<int>();
        var assessmentPrice = _fixture.Create<int>();
        ApprenticeshipDomainModelTestHelper.AddPendingPriceChangeEmployerInitiated(
            apprenticeship, 
            totalPrice: trainingPrice + assessmentPrice,
            effectiveFromDate:apprenticeship.LatestPrice.StartDate.AddDays(_fixture.Create<int>()));
        apprenticeship.ApprovePriceChange("Bob", trainingPrice, assessmentPrice);
        var priceChange = apprenticeship.PriceHistories.First(x => x.PriceChangeRequestStatus == ChangeRequestStatus.Approved);
        var domainEvent = new PriceChangeApproved(apprenticeship.Key, priceChange.Key, ApprovedBy.Provider);

        _apprenticeshipRepository.Setup(x => x.Get(domainEvent.ApprenticeshipKey)).ReturnsAsync(apprenticeship);

        await _handler.Handle(domainEvent);

        _messageSession.Verify(x => x.Publish(It.Is<ApprenticeshipPriceChangedEvent>(e =>
            DoApprenticeshipDetailsMatchDomainModel(e, apprenticeship) && 
            SendsApproverAndInittorDetails(e, priceChange) &&
            IsMarkedApprovedByProvider(e, priceChange) &&
            e.EffectiveFromDate == priceChange.EffectiveFromDate &&
            ApprenticeshipDomainModelTestHelper.DoEpisodeDetailsMatchDomainModel(e, apprenticeship)), It.IsAny<PublishOptions>()));
    }

    private static bool IsMarkedApprovedByEmployer(ApprenticeshipPriceChangedEvent e, PriceHistoryDomainModel priceChange)
    {
        return
            e.ApprovedDate == priceChange.EmployerApprovedDate!.Value &&
            e.ApprovedBy == ApprovedBy.Employer;
    }

    private static bool IsMarkedApprovedByProvider(ApprenticeshipPriceChangedEvent e, PriceHistoryDomainModel priceChange)
    {
        return
            e.ApprovedDate == priceChange.ProviderApprovedDate!.Value &&
            e.ApprovedBy == ApprovedBy.Provider;
    }
        
    private static bool DoApprenticeshipDetailsMatchDomainModel(ApprenticeshipPriceChangedEvent e, ApprenticeshipDomainModel apprenticeship)
    {
        return
            e.ApprenticeshipKey == apprenticeship.Key &&
            e.ApprenticeshipId == apprenticeship.ApprovalsApprenticeshipId;
    }

    private static bool SendsApproverAndInittorDetails(ApprenticeshipPriceChangedEvent e, PriceHistoryDomainModel priceChange)
    {
        return
            e.ProviderApprovedBy == priceChange.ProviderApprovedBy &&
            e.EmployerApprovedBy == priceChange.EmployerApprovedBy;
    }
}