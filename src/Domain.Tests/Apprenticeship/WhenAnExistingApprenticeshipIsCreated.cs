using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Models;
using SFA.DAS.Apprenticeships.Domain.Factories;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Apprenticeship
{
    [TestFixture]
    public class WhenAnExistingApprenticeshipIsCreated
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
            var expectedModel = _fixture.Create<ApprenticeshipModel>();
            var apprenticeship = _apprenticeshipFactory.GetExisting(expectedModel);
            apprenticeship.GetEntity().Should().BeSameAs(expectedModel);
        }
    }
}
