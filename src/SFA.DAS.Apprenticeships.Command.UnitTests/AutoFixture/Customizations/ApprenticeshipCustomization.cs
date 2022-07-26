using System;
using AutoFixture;
using SFA.DAS.Apprenticeships.Domain.Factories;

namespace SFA.DAS.Apprenticeships.Command.UnitTests.AutoFixture.Customizations
{
    public class ApprenticeshipCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Register(() => new ApprenticeshipFactory().CreateNew(fixture.Create<long>(), fixture.Create<string>()));
        }
    }
}
