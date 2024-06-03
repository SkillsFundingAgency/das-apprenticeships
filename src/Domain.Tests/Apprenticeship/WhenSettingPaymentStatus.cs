using AutoFixture;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Factories;
using SFA.DAS.Apprenticeships.Domain.UnitTests.Helpers;
using SFA.DAS.Apprenticeships.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Apprenticeship;

public class WhenSettingPaymentStatus
{
    private Fixture _fixture;
    private ApprenticeshipFactory _apprenticeshipFactory;
    private const string _userId = "AnyUserId";

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
        var apprenticeship = CreateApprenticeshipDomainModel();

        //Act / Assert
        var exception = Assert.Throws<InvalidOperationException>(()=>apprenticeship.SetPaymentStatus(false, _userId, DateTime.Now));
        Assert.That(exception.Message, Is.EqualTo($"Payments are already unfrozen for this apprenticeship: {apprenticeship.Key}."));

    }

    [Test]
    public void And_NewStatusIsFrozen_Then_PaymentsAreFrozen()
    {
        //Arrange
        var apprenticeship = CreateApprenticeshipDomainModel();

        //Act 
        apprenticeship.SetPaymentStatus(true, _userId, DateTime.Now);

        //Assert
        Assert.That(apprenticeship.PaymentsFrozen.Equals(true));
        Assert.That(apprenticeship.FreezeRequests.Count.Equals(1));
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
