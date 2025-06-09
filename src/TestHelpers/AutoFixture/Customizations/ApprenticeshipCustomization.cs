using AutoFixture;
using SFA.DAS.Learning.Domain.Factories;

namespace SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations
{
    public class ApprenticeshipCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Register(() => new LearningFactory().CreateNew(
                fixture.Create<long>(),
                fixture.Create<string>(),
                fixture.Create<DateTime>(),
                fixture.Create<string>(),
                fixture.Create<string>(),
                fixture.Create<string>()));
        }
    }
}
