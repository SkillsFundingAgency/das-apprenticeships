using System;
using System.IO;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SFA.DAS.Apprenticeships.Command;
using SFA.DAS.Apprenticeships.Functions;
using SFA.DAS.Apprenticeships.Infrastructure;
using SFA.DAS.Apprenticeships.Infrastructure.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;

[assembly: FunctionsStartup(typeof(Startup))]
namespace SFA.DAS.Apprenticeships.Functions
{
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
                .AddEnvironmentVariables()
                .AddJsonFile("local.settings.json", optional: true);

            if (!configuration["EnvironmentName"].Equals("LOCAL_ACCEPTANCE_TESTS", StringComparison.CurrentCultureIgnoreCase))
            {
                configBuilder.AddAzureTableStorage(options =>
                {
                    options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
                    options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                    options.EnvironmentName = configuration["EnvironmentName"];
                    options.PreFixConfigurationKeys = false;
                });
            }

            Configuration = configBuilder.Build();

            var applicationSettings = new ApplicationSettings();
            Configuration.Bind(nameof(ApplicationSettings), applicationSettings);
            EnsureConfig(applicationSettings);
            Environment.SetEnvironmentVariable("NServiceBusConnectionString", applicationSettings.NServiceBusConnectionString);

            builder.Services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), Configuration));
            builder.Services.AddSingleton(_ => applicationSettings);

            builder.Services.AddNServiceBus(applicationSettings);

            builder.Services.AddOptions();

            builder.Services.AddEntityFrameworkForApprenticeships();
            builder.Services.AddCommandServices();
        }

        private static void EnsureConfig(ApplicationSettings applicationSettings) // TODO: Delete this
        {
            if (string.IsNullOrWhiteSpace(applicationSettings.NServiceBusConnectionString))
                throw new Exception("NServiceBusConnectionString in ApplicationSettings should not be null.");
        }
    }
}
