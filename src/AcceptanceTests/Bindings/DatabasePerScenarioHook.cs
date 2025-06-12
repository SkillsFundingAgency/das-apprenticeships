using SFA.DAS.Learning.TestHelpers;

namespace SFA.DAS.Learning.AcceptanceTests.Bindings
{
    [Binding]
    public class DatabasePerScenarioHook
    {
        [BeforeScenario(Order = 2)]
        public void CreateDatabase(TestContext context)
        {
            context.SqlDatabase = new SqlDatabase();
        }

        [AfterScenario(Order = 100)]
        public static void TearDownDatabase(TestContext context)
        {
            context.SqlDatabase?.Dispose();
        }
    }
}
