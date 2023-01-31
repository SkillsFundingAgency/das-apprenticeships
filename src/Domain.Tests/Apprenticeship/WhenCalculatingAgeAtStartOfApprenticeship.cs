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
        private Domain.Apprenticeship.Apprenticeship _apprenticeship;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            var apprenticeshipFactory = new ApprenticeshipFactory();
            _apprenticeship = apprenticeshipFactory.CreateNew("1234435", "TRN", new DateTime(2000, 10, 16));

            _fixture = new Fixture();
        }

        [Test]
        public void ThenEarliestApprovalStartDateIsUsed()
        {
            _apprenticeship.AddApproval(_fixture.Create<long>(), _fixture.Create<long>(), _fixture.Create<long>(), _fixture.Create<string>(), new DateTime(2022, 09, 01), _fixture.Create<DateTime>(), _fixture.Create<decimal>(), _fixture.Create<long>(), _fixture.Create<FundingType>(), _fixture.Create<int>(), _fixture.Create<DateTime?>(), false);
            _apprenticeship.AddApproval(_fixture.Create<long>(), _fixture.Create<long>(), _fixture.Create<long>(), _fixture.Create<string>(), new DateTime(2020, 11, 01), _fixture.Create<DateTime>(), _fixture.Create<decimal>(), _fixture.Create<long>(), _fixture.Create<FundingType>(), _fixture.Create<int>(), _fixture.Create<DateTime?>(), false);

            _apprenticeship.AgeAtStartOfApprenticeship.Should().Be(20);
        }
        
        [Test]
        public void WhenTheStartDateIsLaterInTheYearThenTheDateOfBirthThenAgeIsCorrect()
        {
            _apprenticeship.AddApproval(_fixture.Create<long>(), _fixture.Create<long>(), _fixture.Create<long>(), _fixture.Create<string>(), new DateTime(2020, 11, 01), _fixture.Create<DateTime>(), _fixture.Create<decimal>(), _fixture.Create<long>(), _fixture.Create<FundingType>(), _fixture.Create<int>(), _fixture.Create<DateTime?>(), false);

            _apprenticeship.AgeAtStartOfApprenticeship.Should().Be(20);
        }

        [Test]
        public void WhenTheStartDateIsEarlierInTheYearThenTheDateOfBirthThenAgeIsCorrect()
        {
            _apprenticeship.AddApproval(_fixture.Create<long>(), _fixture.Create<long>(), _fixture.Create<long>(), _fixture.Create<string>(), new DateTime(2020, 09, 01), _fixture.Create<DateTime>(), _fixture.Create<decimal>(), _fixture.Create<long>(), _fixture.Create<FundingType>(), _fixture.Create<int>(), _fixture.Create<DateTime?>(), false);

            _apprenticeship.AgeAtStartOfApprenticeship.Should().Be(19);
        }

        [Test]
        public void WhenCalculatingForAnApprenticeWithSldFundingTypeThenAgeShouldBeNull() //TODO: check this change is correct
        {
            _apprenticeship.AddApproval(_fixture.Create<long>(), _fixture.Create<long>(), _fixture.Create<long>(), _fixture.Create<string>(), null, _fixture.Create<DateTime>(), _fixture.Create<decimal>(), _fixture.Create<long>(), _fixture.Create<FundingType>(), _fixture.Create<int>(), new DateTime(2020, 11, 01), true);

            _apprenticeship.AgeAtStartOfApprenticeship.Should().Be(null);
        }
    }
}
