using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Domain.Factories;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Factories
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
            var expectedModel = _fixture.Create<DataAccess.Entities.Apprenticeship.Apprenticeship>();
            var apprenticeship = _apprenticeshipFactory.GetExisting(expectedModel);
            apprenticeship.GetEntity().Should().BeSameAs(expectedModel);
        }
    }
}
