using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Apprenticeships.Functions;
using SFA.DAS.Apprenticeships.Infrastructure.Configuration;
using SFA.DAS.Apprenticeships.TestHelpers;

namespace SFA.DAS.Apprenticeships.AcceptanceTests;

public class Settings
{
    public string AzureWebJobsStorage { get; set; }
    public string NServiceBusConnectionString { get; set; }
    public string TopicPath { get; set; }
    public string QueueName { get; set; }
    public string DbConnectionString { get; set; }
}

public class TestFunction : IDisposable
{
    private readonly TestContext _testContext;
    private readonly IHost _host;
    private bool _isDisposed;

    private IJobHost Jobs => _host.Services.GetService<IJobHost>();
    public string HubName { get; }

    public TestFunction(TestContext testContext, string hubName)
    {
        HubName = hubName;
        var orchestrationData = new OrchestrationData();

        _testContext = testContext;

        
        var config = new ConfigurationBuilder()
            //.SetBasePath()
            .AddJsonFile("local.settings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var settings = new Settings();

        config.Bind(settings);

        var appConfig = new Dictionary<string, string>{
            { "EnvironmentName", "LOCAL_ACCEPTANCE_TESTS" },
            { "AzureWebJobsStorage", settings.AzureWebJobsStorage },
            { "NServiceBusConnectionString", settings.NServiceBusConnectionString ?? "UseLearningEndpoint=true" },
            { "TopicPath", settings.TopicPath },
            { "QueueName", settings.QueueName },
            { "DbConnectionString", _testContext.SqlDatabase?.DatabaseInfo.ConnectionString }
        };

        _testContext = testContext;

        Environment.SetEnvironmentVariable("AzureWebJobsStorage", "UseDevelopmentStorage=true", EnvironmentVariableTarget.Process);
        Environment.SetEnvironmentVariable("NServiceBusConnectionString", "UseLearningEndpoint=true", EnvironmentVariableTarget.Process);
        Environment.SetEnvironmentVariable("ApplicationSettings:NServiceBusConnectionString", "UseLearningEndpoint=true", EnvironmentVariableTarget.Process);
        Environment.SetEnvironmentVariable("LearningTransportStorageDirectory", Path.Combine(Directory.GetCurrentDirectory().Substring(0, Directory.GetCurrentDirectory().IndexOf("src")), @"src\.learningtransport"), EnvironmentVariableTarget.Process);
        Environment.SetEnvironmentVariable("EnvironmentName", "LOCAL_ACCEPTANCE_TESTS", EnvironmentVariableTarget.Process);
        Environment.SetEnvironmentVariable("DbConnectionString", _testContext.SqlDatabase?.DatabaseInfo.ConnectionString, EnvironmentVariableTarget.Process);

        _host = new HostBuilder()
            .ConfigureAppConfiguration(a =>
            {
                a.Sources.Clear();
                a.AddInMemoryCollection(appConfig);
            })
            .ConfigureWebJobs(builder => builder
                .AddDurableTask(options =>
                {
                    options.HubName = HubName;
                    options.UseAppLease = false;
                    options.UseGracefulShutdown = false;
                    options.ExtendedSessionsEnabled = false;
                    options.StorageProvider["maxQueuePollingInterval"] = new TimeSpan(0, 0, 0, 0, 500);
                    options.StorageProvider["partitionCount"] = 1;
                })
                .AddAzureStorageCoreServices()
                .ConfigureServices(s =>
                {
                    s.Configure<ApplicationSettings>(a =>
                    {
                        a.AzureWebJobsStorage = appConfig["AzureWebJobsStorage"];
                        a.QueueName = appConfig["QueueName"];
                        a.TopicPath = appConfig["TopicPath"];
                        a.NServiceBusConnectionString = appConfig["NServiceBusConnectionString"];
                        a.DbConnectionString = appConfig["DbConnectionString"];
                    });

                    new Startup().Configure(builder);

                    s.AddSingleton(typeof(IOrchestrationData), orchestrationData);
                })
            )
            .ConfigureServices(s =>
            {
                s.AddHostedService<PurgeBackgroundJob>();
            })
            .Build();
    }

    public async Task StartHost()
    {
        var timeout = new TimeSpan(0, 2, 10);
        var delayTask = Task.Delay(timeout);

        await Task.WhenAny(Task.WhenAll(_host.StartAsync(), Jobs.Terminate()), delayTask);

        if (delayTask.IsCompleted)
        {
            throw new Exception($"Failed to start test function host within {timeout.Seconds} seconds.  Check the AzureStorageEmulator is running. ");
        }
    }
    
    public async Task DisposeAsync()
    {
        await Jobs.StopAsync();
        Dispose();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed) return;

        if (disposing)
        {
            _host.Dispose();
        }

        _isDisposed = true;
    }
}