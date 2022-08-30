using System.Diagnostics.CodeAnalysis;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using SFA.DAS.Apprenticeships.Infrastructure.Configuration;
using SFA.DAS.NServiceBus.AzureFunction.Hosting;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using System.Text.RegularExpressions;

namespace SFA.DAS.Apprenticeships.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public static class NServiceBusStartupExtensions
    {
        public static IServiceCollection AddNServiceBus(
            this IServiceCollection serviceCollection,
            ApplicationSettings applicationSettings)
        {
            var webBuilder = serviceCollection.AddWebJobs(x => { });
            webBuilder.AddExecutionContextBinding();
            webBuilder.AddExtension(new NServiceBusExtensionConfigProvider());

            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.Apprenticeships")
                .UseMessageConventions()
                .UseNewtonsoftJsonSerializer();

            endpointConfiguration.SendOnly();

            if (applicationSettings.NServiceBusConnectionString.Equals("UseLearningEndpoint=true", StringComparison.CurrentCultureIgnoreCase))
            {
                var learningTransportFolder = string.IsNullOrEmpty(applicationSettings.LearningTransportStorageDirectory) ?
                    Path.Combine(
                        Directory.GetCurrentDirectory()[..Directory.GetCurrentDirectory().IndexOf("src", StringComparison.Ordinal)],
                        @"src\.learningtransport")
                    : applicationSettings.LearningTransportStorageDirectory;
                endpointConfiguration
                    .UseTransport<LearningTransport>()
                    .StorageDirectory(learningTransportFolder);
                endpointConfiguration.UseLearningTransport(s => s.AddRouting());
                Environment.SetEnvironmentVariable("LearningTransportStorageDirectory", learningTransportFolder, EnvironmentVariableTarget.Process);
            }
            else
            {
                endpointConfiguration
                    .UseAzureServiceBusTransport(applicationSettings.NServiceBusConnectionString, r => r.AddRouting());
            }

            if (!string.IsNullOrEmpty(applicationSettings.NServiceBusLicense))
            {
                endpointConfiguration.License(applicationSettings.NServiceBusLicense);
            }

            ExcludeTestAssemblies(endpointConfiguration.AssemblyScanner());

            endpointConfiguration.UseEndpointWithExternallyManagedService(serviceCollection);

            return serviceCollection;
        }

        private static void ExcludeTestAssemblies(AssemblyScannerConfiguration scanner)
        {
            var excludeRegexs = new List<string>
            {
                @"nunit.*.dll"
            };

            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            foreach (var fileName in Directory.EnumerateFiles(baseDirectory, "*.dll")
                         .Select(Path.GetFileName))
            {
                foreach (var pattern in excludeRegexs)
                {
                    if (Regex.IsMatch(fileName, pattern, RegexOptions.IgnoreCase))
                    {
                        scanner.ExcludeAssemblies(fileName);
                        break;
                    }
                }
            }
        }
    }
}