﻿using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SFA.DAS.Apprenticeships.Command;
using SFA.DAS.Apprenticeships.Domain;
using SFA.DAS.Apprenticeships.Functions;
using SFA.DAS.Apprenticeships.Infrastructure;
using SFA.DAS.Apprenticeships.Infrastructure.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.Extensions.Logging;
using SFA.DAS.Apprenticeships.DataAccess;

[assembly: FunctionsStartup(typeof(Startup))]
namespace SFA.DAS.Apprenticeships.Functions;

[ExcludeFromCodeCoverage]
public class Startup : FunctionsStartup
{
    public IConfiguration Configuration { get; set; }

    public override void Configure(IFunctionsHostBuilder builder)
    {
        var serviceProvider = builder.Services.BuildServiceProvider();
        var configuration = serviceProvider.GetService<IConfiguration>();

        var configBuilder = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables();

        if (NotAcceptanceTests(configuration))
        {
            configBuilder.AddAzureTableStorage(options =>
            {
                options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
                options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                options.EnvironmentName = configuration["EnvironmentName"];
                options.PreFixConfigurationKeys = false;
                options.ConfigurationKeysRawJsonResult = new[] { "SFA.DAS.Encoding" };
            });
            configBuilder.AddJsonFile("local.settings.json", optional: true);
        }

        Configuration = configBuilder.Build();
        builder.Services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), Configuration));
        builder.Services.AddOptions();

        builder.Services.Configure<ApplicationSettings>(Configuration.GetSection("ApplicationSettings"));
        var applicationSettings = Configuration.GetSection("ApplicationSettings").Get<ApplicationSettings>();

        Environment.SetEnvironmentVariable("NServiceBusConnectionString", applicationSettings.NServiceBusConnectionString);

        builder.Services.AddNServiceBus(applicationSettings);
        builder.Services.AddEntityFrameworkForApprenticeships(applicationSettings, NotLocal(configuration));

        builder.Services.AddCommandServices(Configuration).AddEventServices();

        if(NotAcceptanceTests(configuration))
            builder.Services.AddApprenticeshipsOuterApiClient(applicationSettings.ApprenticeshipsOuterApiConfiguration.BaseUrl, applicationSettings.ApprenticeshipsOuterApiConfiguration.Key);

        builder.Services.AddLogging((options) =>
        {
            options.AddFilter("SFA.DAS", LogLevel.Debug); // this is because all logging is filtered out by default
            options.SetMinimumLevel(LogLevel.Trace);
        });

    }

    private static bool NotAcceptanceTests(IConfiguration configuration)
    {
        return !configuration!["EnvironmentName"].Equals("LOCAL_ACCEPTANCE_TESTS", StringComparison.CurrentCultureIgnoreCase);
    }

    private static bool NotLocal(IConfiguration configuration)
    {
        var env = configuration!["EnvironmentName"];
        var isLocal = env.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase);
        var isLocalAcceptanceTests = env.Equals("LOCAL_ACCEPTANCE_TESTS", StringComparison.CurrentCultureIgnoreCase);
        return !isLocal && !isLocalAcceptanceTests;
    }
}