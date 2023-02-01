using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Events;
using SFA.DAS.Apprenticeships.Domain.Factories;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Apprenticeship
{
    [TestFixture]
    public class WhenAnApprenticeshipIsCreated
    {
        private ApprenticeshipFactory _apprenticeshipFactory;

        [SetUp]
        public void SetUp()
        {
            _apprenticeshipFactory = new ApprenticeshipFactory();
        }

        [Test]
        public void ThenAnApprenticeshipCreatedEventIsAdded()
        {
            var apprenticeship = _apprenticeshipFactory.CreateNew("1234567", "TRCODE", DateTime.Now, "Ron", "Swanson");
            var events = apprenticeship.FlushEvents();
            events.Should().ContainSingle(x => x.GetType() == typeof(ApprenticeshipCreated));
        }
    }
}
