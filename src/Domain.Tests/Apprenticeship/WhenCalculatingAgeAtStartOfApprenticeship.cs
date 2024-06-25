using System;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Factories;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Apprenticeship
{
    [TestFixture]
    public class WhenCalculatingAgeAtStartOfApprenticeship
    {
        private Fixture _fixture;

        public WhenCalculatingAgeAtStartOfApprenticeship()
        {
            _fixture = new Fixture();
            _fixture.Customize(new ApprenticeshipCustomization());
        }
        
        [Test]
        public void WhenTheStartDateIsLaterInTheYearThenTheDateOfBirthThenAgeIsCorrect()
        {
            var dateOfBirth = new DateTime(2000, 10, 16);
            var startDate = new DateTime(2020, 11, 01);
            var apprenticeship = CreateApprenticeshipDomainModel(dateOfBirth, startDate, FundingPlatform.DAS);

            apprenticeship.AgeAtStartOfApprenticeship.Should().Be(20);
        }

        [Test]
        public void WhenTheStartDateIsEarlierInTheYearThenTheDateOfBirthThenAgeIsCorrect()
        {
            var dateOfBirth = new DateTime(2000, 10, 16);
            var startDate = new DateTime(2020, 09, 01);
            var apprenticeship = CreateApprenticeshipDomainModel(dateOfBirth, startDate, FundingPlatform.DAS);

            apprenticeship.AgeAtStartOfApprenticeship.Should().Be(19);
        }

        [Test]
        public void WhenCalculatingForAnApprenticeWithSldFundingTypeThenAgeShouldBeNull()
        {
            var dateOfBirth = new DateTime(2000, 10, 16);
            var startDate = _fixture.Create<DateTime>();
            var apprenticeship = CreateApprenticeshipDomainModel(dateOfBirth, startDate, FundingPlatform.SLD);

            apprenticeship.AgeAtStartOfApprenticeship.Should().Be(null);
        }

        private ApprenticeshipDomainModel CreateApprenticeshipDomainModel(DateTime dateOfBirth, DateTime startDate, FundingPlatform fundingPlatform)
        {
            var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
            apprenticeship.AddEpisode(
                _fixture.Create<long>(), 
                _fixture.Create<long>(),
                startDate,
                _fixture.Create<DateTime>(), 
                _fixture.Create<decimal>(), 
                _fixture.Create<decimal?>(), 
                _fixture.Create<decimal?>(), 
                _fixture.Create<FundingType>(), 
                fundingPlatform,
                _fixture.Create<int>(), 
                _fixture.Create<long?>(), 
                _fixture.Create<string>(), 
                _fixture.Create<long?>(), 
                _fixture.Create<string>(),
                _fixture.Create<string>());

            return apprenticeship;

        }
    }
}
