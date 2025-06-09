using SFA.DAS.Apprenticeships.TestHelpers;

namespace SFA.DAS.Learning.AcceptanceTests
{
    public class TestContext : IDisposable
    {
        public TestFunction? TestFunction { get; set; }
        public SqlDatabase? SqlDatabase { get; set; }
        public TestMessageSession MessageSession { get; set; }

        public void Dispose()
        {
            TestFunction?.Dispose();
            SqlDatabase?.Dispose();
        }

        public TestContext()
        {
            MessageSession = new TestMessageSession();
        }
    }
}
