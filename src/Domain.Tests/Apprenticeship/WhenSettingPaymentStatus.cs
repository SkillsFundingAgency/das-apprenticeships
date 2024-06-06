using AutoFixture;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Factories;
using System;
using System.Linq;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Apprenticeship;

public class WhenSettingPaymentStatus
{
    private Fixture _fixture;
    private ApprenticeshipFactory _apprenticeshipFactory;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
        _apprenticeshipFactory = new ApprenticeshipFactory();
    }

    [Test]
    public void And_NewStatusIsSameAsOld_Then_Throws()
    {
        //Arrange
        var userId = _fixture.Create<string>();
        var apprenticeship = CreateApprenticeshipDomainModel();

        //Act / Assert
        var exception = Assert.Throws<InvalidOperationException>(()=>apprenticeship.SetPaymentStatus(false, userId, DateTime.Now));
        Assert.That(exception.Message, Is.EqualTo($"Payments are already unfrozen for this apprenticeship: {apprenticeship.Key}."));

    }

    [Test]
    public void And_NewStatusIsFrozen_Then_PaymentsAreFrozen()
    {
        //Arrange
        var userId = _fixture.Create<string>();
        var timeChanged = DateTime.Now;
        var apprenticeship = CreateApprenticeshipDomainModel();

        //Act 
        apprenticeship.SetPaymentStatus(true, userId, timeChanged);

        //Assert
        Assert.That(apprenticeship.PaymentsFrozen.Equals(true));
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
        var apprenticeship = CreateApprenticeshipDomainModel();
        apprenticeship.SetPaymentStatus(true, userIdFreeze, timefreeze);

        //Act 
        apprenticeship.SetPaymentStatus(false, userIdUnfreeze, timeUnfreeze);

        //Assert
        Assert.That(apprenticeship.PaymentsFrozen.Equals(false));
        Assert.That(apprenticeship.FreezeRequests.Count(x => 
            x.FrozenBy == userIdFreeze && 
            x.FrozenDateTime == timefreeze &&
            x.UnfrozenBy == userIdUnfreeze &&
            x.UnfrozenDateTime == timeUnfreeze &&
            x.Unfrozen), 
            Is.EqualTo(1));
    }

    private ApprenticeshipDomainModel CreateApprenticeshipDomainModel()
    {
        
        return _apprenticeshipFactory.CreateNew(
            "1234435",
            "TRN",
            new DateTime(2000,
                10,
                16),
            "Ron",
            "Swanson",
            _fixture.Create<decimal?>(),
            _fixture.Create<decimal?>(),
            _fixture.Create<decimal>(),
            _fixture.Create<string>(),
            _fixture.Create<int>(),
            _fixture.Create<DateTime>(),
            _fixture.Create<DateTime>(),
            _fixture.Create<long>(),
            _fixture.Create<long>(),
            _fixture.Create<long>(),
            _fixture.Create<string>());
    }
}
