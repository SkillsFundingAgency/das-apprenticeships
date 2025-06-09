namespace SFA.DAS.Learning.AcceptanceTests.Bindings;

[Binding]
public class TestFunctionPerScenarioHook
{
    private readonly TestContext _testContext;
    private readonly FeatureContext _featureContext;

    public TestFunctionPerScenarioHook(TestContext testContext, FeatureContext featureContext)
    {
        _testContext = testContext;
        _featureContext = featureContext;
    }

    [BeforeScenario]
    public async Task CreateConfig()
    {
        _testContext.TestFunction = new TestFunction(_testContext, $"TEST{_featureContext.FeatureInfo.Title.Replace(" ", "")}");
    }

    [AfterScenario]
    public async Task CleanupAfterTestHarness()
    {
        await _testContext.TestFunction?.DisposeAsync()!;
    }
}