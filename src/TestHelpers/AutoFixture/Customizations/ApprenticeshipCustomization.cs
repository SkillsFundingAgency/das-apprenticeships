using AutoFixture;
using SFA.DAS.Apprenticeships.Domain.Factories;

namespace SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations
{
    public class ApprenticeshipCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Register(() => new ApprenticeshipFactory().CreateNew(
                fixture.Create<string>(),
                fixture.Create<DateTime>(),
                fixture.Create<string>(),
                fixture.Create<string>(),
                fixture.Create<string>()));
        }
    }
}
