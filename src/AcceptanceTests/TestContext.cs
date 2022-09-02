using SFA.DAS.Apprenticeships.TestHelpers;

namespace SFA.DAS.Apprenticeships.AcceptanceTests
{
    public class TestContext : IDisposable
    {
        public TestFunction? TestFunction { get; set; }
        public SqlDatabase? SqlDatabase { get; set; }

        public void Dispose()
        {
            TestFunction?.Dispose();
            SqlDatabase?.Dispose();
        }
    }
}
