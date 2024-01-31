using System;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Domain.Factories;
using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Apprenticeship
{
    [TestFixture]
    public class WhenCalculatingAgeAtStartOfApprenticeship
    {
        private Domain.Apprenticeship.ApprenticeshipDomainModel _apprenticeship;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            var apprenticeshipFactory = new ApprenticeshipFactory();
            _fixture = new Fixture();
            _apprenticeship = apprenticeshipFactory.CreateNew(
                "1234435", 
                "TRN", 
                new DateTime(2000, 10, 16), 
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
                _fixture.Create<long>());
        }

        [Test]
        public void ThenEarliestApprovalStartDateIsUsed()
        {
            _apprenticeship.AddApproval(_fixture.Create<long>(), _fixture.Create<long>(), _fixture.Create<long>(), _fixture.Create<string>(), new DateTime(2022, 09, 01), _fixture.Create<DateTime>(), _fixture.Create<decimal>(), _fixture.Create<long>(), _fixture.Create<FundingType>(), _fixture.Create<int>(), _fixture.Create<DateTime?>(), FundingPlatform.SLD);
            _apprenticeship.AddApproval(_fixture.Create<long>(), _fixture.Create<long>(), _fixture.Create<long>(), _fixture.Create<string>(), new DateTime(2020, 11, 01), _fixture.Create<DateTime>(), _fixture.Create<decimal>(), _fixture.Create<long>(), _fixture.Create<FundingType>(), _fixture.Create<int>(), _fixture.Create<DateTime?>(), FundingPlatform.SLD);

            _apprenticeship.AgeAtStartOfApprenticeship.Should().Be(20);
        }
        
        [Test]
        public void WhenTheStartDateIsLaterInTheYearThenTheDateOfBirthThenAgeIsCorrect()
        {
            _apprenticeship.AddApproval(_fixture.Create<long>(), _fixture.Create<long>(), _fixture.Create<long>(), _fixture.Create<string>(), new DateTime(2020, 11, 01), _fixture.Create<DateTime>(), _fixture.Create<decimal>(), _fixture.Create<long>(), _fixture.Create<FundingType>(), _fixture.Create<int>(), _fixture.Create<DateTime?>(), FundingPlatform.SLD);

            _apprenticeship.AgeAtStartOfApprenticeship.Should().Be(20);
        }

        [Test]
        public void WhenTheStartDateIsEarlierInTheYearThenTheDateOfBirthThenAgeIsCorrect()
        {
            _apprenticeship.AddApproval(_fixture.Create<long>(), _fixture.Create<long>(), _fixture.Create<long>(), _fixture.Create<string>(), new DateTime(2020, 09, 01), _fixture.Create<DateTime>(), _fixture.Create<decimal>(), _fixture.Create<long>(), _fixture.Create<FundingType>(), _fixture.Create<int>(), _fixture.Create<DateTime?>(), FundingPlatform.SLD);

            _apprenticeship.AgeAtStartOfApprenticeship.Should().Be(19);
        }

        [Test]
        public void WhenCalculatingForAnApprenticeWithSldFundingTypeThenAgeShouldBeNull()
        {
            _apprenticeship.AddApproval(_fixture.Create<long>(), _fixture.Create<long>(), _fixture.Create<long>(), _fixture.Create<string>(), null, _fixture.Create<DateTime>(), _fixture.Create<decimal>(), _fixture.Create<long>(), _fixture.Create<FundingType>(), _fixture.Create<int>(), new DateTime(2020, 11, 01), FundingPlatform.DAS);

            _apprenticeship.AgeAtStartOfApprenticeship.Should().Be(null);
        }
    }
}
