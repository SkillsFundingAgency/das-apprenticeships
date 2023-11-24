using System;
using AutoFixture;
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
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _apprenticeshipFactory = new ApprenticeshipFactory();
            _fixture = new Fixture();
        }

        [Test]
        public void ThenAnApprenticeshipCreatedEventIsAdded()
        {
            var apprenticeship = _apprenticeshipFactory.CreateNew(
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
                _fixture.Create<long>());
            var events = apprenticeship.FlushEvents();
            events.Should().ContainSingle(x => x.GetType() == typeof(ApprenticeshipCreated));
        }
    }
}
