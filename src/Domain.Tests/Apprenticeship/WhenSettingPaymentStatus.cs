using AutoFixture;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using System;
using SFA.DAS.Apprenticeships.Domain.UnitTests.Helpers;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;
using SFA.DAS.Apprenticeships.Domain.Factories;
using System.Linq;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Apprenticeship;

public class WhenSettingPaymentStatus
{
    private Fixture _fixture;
    private const string _userId = "AnyUserId";
    private ApprenticeshipFactory _apprenticeshipFactory;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
        _fixture.Customize(new ApprenticeshipCustomization());
    }

    [Test]
    public void And_NewStatusIsSameAsOld_Then_Throws()
    {
        //Arrange
        var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
        ApprenticeshipDomainModelTestHelper.AddEpisode(apprenticeship);
        var userId = _fixture.Create<string>();

        //Act / Assert
        var exception = Assert.Throws<InvalidOperationException>(()=>apprenticeship.SetPaymentsFrozen(false, userId, DateTime.Now));
        Assert.That(exception.Message, Is.EqualTo($"Payments are already unfrozen for this apprenticeship: {apprenticeship.Key}."));

    }

    [Test]
    public void And_NewStatusIsFrozen_Then_PaymentsAreFrozen()
    {
        //Arrange
        var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
        ApprenticeshipDomainModelTestHelper.AddEpisode(apprenticeship);
        var userId = _fixture.Create<string>();
        var timeChanged = DateTime.Now;

        //Act 
        apprenticeship.SetPaymentsFrozen(true, userId, timeChanged);

        //Assert
        Assert.That(apprenticeship.LatestEpisode.PaymentsFrozen.Equals(true));
        Assert.That(apprenticeship.FreezeRequests.Count.Equals(1));
        Assert.That(apprenticeship.FreezeRequests.Count(x => 
            x.FrozenBy == userId && 
            x.FrozenDateTime == timeChanged && 
            !x.Unfrozen), 
            Is.EqualTo(1));
    }

    [Test]
    public void And_NewStatusIsUnfreeze_Then_PaymentsAreActive()
    {
        //Arrange
        var userIdFreeze = _fixture.Create<string>();
        var userIdUnfreeze = _fixture.Create<string>();
        var timefreeze = DateTime.Now.AddMinutes(-10);
        var timeUnfreeze = DateTime.Now;
        var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
        ApprenticeshipDomainModelTestHelper.AddEpisode(apprenticeship);
        apprenticeship.SetPaymentsFrozen(true, userIdFreeze, timefreeze);

        //Act 
        apprenticeship.SetPaymentsFrozen(false, userIdUnfreeze, timeUnfreeze);

        //Assert
        Assert.That(apprenticeship.LatestEpisode.PaymentsFrozen.Equals(false));
        Assert.That(apprenticeship.FreezeRequests.Count(x => 
            x.FrozenBy == userIdFreeze && 
            x.FrozenDateTime == timefreeze &&
            x.UnfrozenBy == userIdUnfreeze &&
            x.UnfrozenDateTime == timeUnfreeze &&
            x.Unfrozen), 
            Is.EqualTo(1));
    }
}
