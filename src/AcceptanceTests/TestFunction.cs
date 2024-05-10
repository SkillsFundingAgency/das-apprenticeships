using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using SFA.DAS.Apprenticeships.Functions;
using SFA.DAS.Apprenticeships.Infrastructure.ApprenticeshipsOuterApiClient;
using SFA.DAS.Apprenticeships.Infrastructure.Configuration;
using SFA.DAS.Apprenticeships.TestHelpers;
using SFA.DAS.Encoding;
using System;

namespace SFA.DAS.Apprenticeships.AcceptanceTests;

public class TestFunction : IDisposable
{
    private readonly IHost _host;
    private bool _isDisposed;

    private IJobHost Jobs => _host.Services.GetService<IJobHost>()!;
    public string HubName { get; }
    public Mock<IApprenticeshipsOuterApiClient> mockApprenticeshipsOuterApiClient  { get; }

    public TestFunction(TestContext testContext, string hubName)
    {
        HubName = hubName;
        var orchestrationData = new OrchestrationData();

        var appConfig = new Dictionary<string, string>
        {
            { "EnvironmentName", "LOCAL_ACCEPTANCE_TESTS" },
            { "FUNCTIONS_WORKER_RUNTIME", "dotnet" },
            { "AzureWebJobsStorage", "UseDevelopmentStorage=true" },
            { "ApplicationSettings:NServiceBusConnectionString", "UseLearningEndpoint=true" },
            { "ApplicationSettings:LogLevel", "DEBUG" },
            { "ApplicationSettings:DbConnectionString", testContext.SqlDatabase?.DatabaseInfo.ConnectionString! },
            { "SFA.DAS.Encoding", MockEncodingConfig() }
        };

        mockApprenticeshipsOuterApiClient = new Mock<IApprenticeshipsOuterApiClient>();
        mockApprenticeshipsOuterApiClient.Setup(x => x.GetStandard(It.IsAny<int>())).ReturnsAsync(new GetStandardResponse { MaxFunding = int.MaxValue, ApprenticeshipFunding = new List<GetStandardFundingResponse>
        {
            new GetStandardFundingResponse{ EffectiveFrom = DateTime.MinValue, EffectiveTo = null, MaxEmployerLevyCap = int.MaxValue }
        }});

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
                    builder.Services.AddLogging(options =>
                    {
                        options.SetMinimumLevel(LogLevel.Trace);
                        options.AddConsole();
                    });
                    s.Configure<ApplicationSettings>(a =>
                    {
                        a.AzureWebJobsStorage = appConfig["AzureWebJobsStorage"];
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
                s.AddScoped<IApprenticeshipsOuterApiClient>(_ => mockApprenticeshipsOuterApiClient.Object);
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

    private string MockEncodingConfig()
    {
        var config = new List<object>();

		foreach (var encodingType in Enum.GetValues(typeof(EncodingType)))
		{
            config.Add(new { EncodingType = encodingType, Salt = "AnyString", MinHashLength = 6, Alphabet = "ABCDEFGHJKMNPRSTUVWXYZ23456789" });
		}

        return JsonConvert.SerializeObject(new { Encodings = config });
	}
}