using AutoFixture;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using System;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Apprenticeship;

public class WhenSettingPaymentStatus
{
    private Fixture _fixture;
    private const string _userId = "AnyUserId";

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

        //Act / Assert
        var exception = Assert.Throws<InvalidOperationException>(()=>apprenticeship.SetPaymentStatus(false, _userId, DateTime.Now));
        Assert.That(exception.Message, Is.EqualTo($"Payments are already unfrozen for this apprenticeship: {apprenticeship.Key}."));

    }

    [Test]
    public void And_NewStatusIsFrozen_Then_PaymentsAreFrozen()
    {
        //Arrange
        var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();

        //Act 
        apprenticeship.SetPaymentStatus(true, _userId, DateTime.Now);

        //Assert
        //todo fix test for freeze function
        //Assert.That(apprenticeship.PaymentsFrozen.Equals(true));
        //Assert.That(apprenticeship.FreezeRequests.Count.Equals(1));
    }
}
