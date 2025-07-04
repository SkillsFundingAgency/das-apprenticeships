﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using SFA.DAS.Encoding;
using SFA.DAS.Learning.AcceptanceTests.Helpers;
using SFA.DAS.Learning.Functions;
using SFA.DAS.Learning.Infrastructure.ApprenticeshipsOuterApiClient;

namespace SFA.DAS.Learning.AcceptanceTests;

internal class TestFunctionStartup
{
    private readonly Startup _startUp;
    private readonly IEnumerable<MessageHandler> _queueTriggeredFunctions;
    private readonly IMessageSession _messageSession;
    private readonly Mock<IApprenticeshipsOuterApiClient> _mockApprenticeshipsOuterApiClient;

    public TestFunctionStartup(
        TestContext testContext,
        IEnumerable<MessageHandler> queueTriggeredFunctions,
        IMessageSession messageSession,
        Mock<IApprenticeshipsOuterApiClient> mockApprenticeshipsOuterApiClient)
    {
        _startUp = new Startup();
        _startUp.Configuration = GenerateConfiguration(testContext);
        _queueTriggeredFunctions = queueTriggeredFunctions;
        _messageSession = messageSession;
        _mockApprenticeshipsOuterApiClient = mockApprenticeshipsOuterApiClient;
    }

    public void Configure()
    {
        // Intentionally left blank
    }

    public void ConfigureServices(IServiceCollection collection)
    {
        _startUp.SetupServices(collection);

        collection.AddSingleton<IMessageSession>(_messageSession);

        foreach (var queueTriggeredFunction in _queueTriggeredFunctions)
        {
            collection.AddTransient(queueTriggeredFunction.HandlerType);
        }

        collection.AddScoped<IApprenticeshipsOuterApiClient>(_ => _mockApprenticeshipsOuterApiClient.Object);
    }

    private static IConfigurationRoot GenerateConfiguration(TestContext testContext)
    {
        var configSource = new MemoryConfigurationSource
        {
            InitialData = new[]
            {
                new KeyValuePair<string, string>("EnvironmentName", "LOCAL_ACCEPTANCE_TESTS"),
                new KeyValuePair<string, string>("AzureWebJobsStorage", "UseDevelopmentStorage=true"),
                new KeyValuePair<string, string>("AzureWebJobsServiceBus", "UseDevelopmentStorage=true"),
                new KeyValuePair<string, string>("ApplicationSettings:NServiceBusConnectionString", "UseLearningEndpoint=true"),
                new KeyValuePair<string, string>("ApplicationSettings:LogLevel", "DEBUG"),
                new KeyValuePair<string, string>("ApplicationSettings:DbConnectionString", testContext.SqlDatabase?.DatabaseInfo.ConnectionString!),
                new KeyValuePair<string, string>("SFA.DAS.Encoding", MockEncodingConfig())
            }
        };

        var provider = new MemoryConfigurationProvider(configSource);
        return new ConfigurationRoot(new List<IConfigurationProvider> { provider });
    }


    private static string MockEncodingConfig()
    {
        var config = new List<object>();

        foreach (var encodingType in Enum.GetValues(typeof(EncodingType)))
        {
            config.Add(new { EncodingType = encodingType, Salt = "AnyString", MinHashLength = 6, Alphabet = "ABCDEFGHJKMNPRSTUVWXYZ23456789" });
        }

        return JsonConvert.SerializeObject(new { Encodings = config });
    }
}