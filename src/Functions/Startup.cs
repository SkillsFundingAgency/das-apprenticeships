using Microsoft.Azure.Functions.Extensions.DependencyInjection;
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

[assembly: FunctionsStartup(typeof(Startup))]
namespace SFA.DAS.Apprenticeships.Functions
{
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
                .AddEnvironmentVariables()
                ;

            if (NotAcceptanceTests(configuration))
            {
                configBuilder.AddAzureTableStorage(options =>
                {
                    options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
                    options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                    options.EnvironmentName = configuration["EnvironmentName"];
                    options.PreFixConfigurationKeys = false;
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
            builder.Services.AddEntityFrameworkForApprenticeships(applicationSettings);

            builder.Services.AddCommandServices().AddEventServices();
            builder.Services.AddApprovalsOuterApiClient(applicationSettings.ApprovalsOuterApiConfiguration.BaseUrl, applicationSettings.ApprovalsOuterApiConfiguration.Key);
        }

        private static bool NotAcceptanceTests(IConfiguration configuration)
        {
            return !configuration!["EnvironmentName"].Equals("LOCAL_ACCEPTANCE_TESTS", StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
