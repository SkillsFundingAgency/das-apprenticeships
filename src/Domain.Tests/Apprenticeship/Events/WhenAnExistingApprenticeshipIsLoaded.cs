using System;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Events;
using SFA.DAS.Apprenticeships.Domain.Factories;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Apprenticeship.Events
{
    [TestFixture]
    public class WhenAnExistingApprenticeshipIsLoaded
    {
        private ApprenticeshipFactory _apprenticeshipFactory;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _apprenticeshipFactory = new ApprenticeshipFactory();
            _fixture = new Fixture();
        }

        [Test]
        public void ThenAnApprenticeshipCreatedEventIsNotAdded()
        {
            var model = _fixture.Create<DataAccess.Entities.Apprenticeship.Apprenticeship>();
            var apprenticeship = _apprenticeshipFactory.GetExisting(model);
            var events = apprenticeship.FlushEvents();
            events.Should().NotContain(x => x.GetType() == typeof(ApprenticeshipCreated));
        }
    }
}
