namespace SFA.DAS.Apprenticeships.AcceptanceTests.Bindings;

[Binding]
public class HostingStepDefinitions
{
    private readonly TestContext _testContext;
    private readonly FeatureContext _featureContext;

    public HostingStepDefinitions(TestContext testContext, FeatureContext featureContext)
    {
        _testContext = testContext;
        _featureContext = featureContext;
    }

    [BeforeScenario]
    public async Task CreateConfig()
    {
        _testContext.TestFunction = new TestFunction(_testContext, $"TEST{_featureContext.FeatureInfo.Title.Replace(" ", "")}");
        await _testContext.TestFunction.StartHost();
    }

    [AfterScenario]
    public async Task CleanupAfterTestHarness()
    {
        await _testContext.TestFunction?.DisposeAsync()!;
    }
}