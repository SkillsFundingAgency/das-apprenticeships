using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Events;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Apprenticeship.Events
{
    [TestFixture]
    public class WhenAnApprenticeshipIsCreated
    {
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _fixture.Customize(new ApprenticeshipCustomization());
        }

        [Test]
        public void ThenAnApprenticeshipCreatedEventIsAdded()
        {
            var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
            var events = apprenticeship.FlushEvents();
            events.Should().ContainSingle(x => x.GetType() == typeof(ApprenticeshipCreated));
        }
    }
}
