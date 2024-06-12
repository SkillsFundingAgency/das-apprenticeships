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
        private ApprenticeshipFactory _apprenticeshipFactory;
        private Fixture _fixture;

        public WhenCalculatingAgeAtStartOfApprenticeship()
        {
            _apprenticeshipFactory = new ApprenticeshipFactory();
            _fixture = new Fixture();
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

        private Domain.Apprenticeship.ApprenticeshipDomainModel CreateApprenticeshipDomainModel(DateTime dateOfBirth, DateTime startDate, FundingPlatform fundingPlatform)
        {
            var apprenticeship = _apprenticeshipFactory.CreateNew(
                "1234435",
                "TRN",
                dateOfBirth,
                "Ron",
                "Swanson",
                _fixture.Create<decimal?>(),
                _fixture.Create<decimal?>(),
                _fixture.Create<decimal>(),
                _fixture.Create<string>(),
                _fixture.Create<int>(),
                startDate,
                _fixture.Create<DateTime>(),
                _fixture.Create<long>(),
                _fixture.Create<long>(),
                _fixture.Create<long>(),
                _fixture.Create<string>());

            apprenticeship.AddApproval(
                _fixture.Create<long>(), 
                _fixture.Create<string>(), 
                new DateTime(2020, 11, 01), 
                _fixture.Create<DateTime>(), 
                _fixture.Create<decimal>(), 
                _fixture.Create<long>(), 
                _fixture.Create<FundingType>(), 
                _fixture.Create<int>(), 
                _fixture.Create<DateTime?>(),
                fundingPlatform);

            return apprenticeship;

        }
    }
}
