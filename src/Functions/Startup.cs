using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.Apprenticeships.Command;
using SFA.DAS.Apprenticeships.DataAccess;
using SFA.DAS.Apprenticeships.Domain;
using SFA.DAS.Apprenticeships.Functions.AppStart;
using SFA.DAS.Apprenticeships.Infrastructure.Configuration;
using SFA.DAS.Apprenticeships.Infrastructure.Extensions;
using SFA.DAS.Apprenticeships.Types;
using SFA.DAS.Configuration.AzureTableStorage;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace SFA.DAS.Apprenticeships.Functions;

[ExcludeFromCodeCoverage]
public class Startup
{
    public IConfiguration Configuration { get; set; }

    private ApplicationSettings _applicationSettings;
    public ApplicationSettings ApplicationSettings
    {
        get
        {
            if (_applicationSettings == null)
            {
                _applicationSettings = new ApplicationSettings();
                Configuration.Bind(nameof(ApplicationSettings), _applicationSettings);
            }
            return _applicationSettings;
        }
    }

    public Startup()
    {
        ForceAssemblyLoad();
    }

    public void Configure(IHostBuilder builder)
    {
        builder
            .ConfigureAppConfiguration(PopulateConfig)
            .ConfigureNServiceBusForSubscribe()
            .ConfigureServices((c, s) =>
            {
                SetupServices(s);
                s.ConfigureNServiceBusForSend(ApplicationSettings.NServiceBusConnectionString.GetFullyQualifiedNamespace());
            });
    }

    private void PopulateConfig(IConfigurationBuilder configurationBuilder)
    {
        Environment.SetEnvironmentVariable("ENDPOINT_NAME", "SFA.DAS.Apprenticeships");

        configurationBuilder.SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables()
                .AddJsonFile("local.settings.json", true);

        var configuration = configurationBuilder.Build();
        configurationBuilder.AddAzureTableStorage(options =>
        {
            options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
            options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
            options.EnvironmentName = configuration["EnvironmentName"];
            options.PreFixConfigurationKeys = false;
            options.ConfigurationKeysRawJsonResult = new[] { "SFA.DAS.Encoding" };
        });

        Configuration = configurationBuilder.Build();
    }

    public void SetupServices(IServiceCollection services)
    {
        services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), Configuration));
        services.AddOptions();

        services.AddEntityFrameworkForApprenticeships(ApplicationSettings, NotLocal(Configuration));

        services.AddCommandServices(Configuration).AddEventServices().AddValidators();

        if (NotAcceptanceTests(Configuration))
            services.AddApprenticeshipsOuterApiClient(ApplicationSettings.ApprenticeshipsOuterApiConfiguration.BaseUrl, ApplicationSettings.ApprenticeshipsOuterApiConfiguration.Key);

        services.AddLogging((options) =>
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

    /// <summary>
    /// This method is used to force the assembly to load so that the NServiceBus assembly scanner can find the events.
    /// This has to be called before builder configuration steps are called as these don't get executed until build() is called.
    /// </summary>
    private static void ForceAssemblyLoad()
    {
        var apprenticeshipEarningsTypes = new ApprenticeshipCreatedEvent();
    }
}