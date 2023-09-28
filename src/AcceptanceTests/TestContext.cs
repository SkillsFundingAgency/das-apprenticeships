using NServiceBus;
using SFA.DAS.Apprenticeships.TestHelpers;

namespace SFA.DAS.Apprenticeships.AcceptanceTests
{
    public class TestContext : IDisposable
    {
        public TestFunction? TestFunction { get; set; }
        public SqlDatabase? SqlDatabase { get; set; }
        public IEndpointInstance EndpointInstance { get; set; }

        public TestContext()
        {
            AssertionOptions.AssertEquivalencyUsing(options =>
            {
                // This is needed to cater for data precision mismatches for db's v .net datetime fields
                options.Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1))).WhenTypeIs<DateTime>();
                return options;
            });
        }

        public void Dispose()
        {
            TestFunction?.Dispose();
            SqlDatabase?.Dispose();
        }
    }
}
