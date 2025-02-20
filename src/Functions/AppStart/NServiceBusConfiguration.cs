using Azure.Identity;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System;
using System.Linq;
using NServiceBus;
using System.Net;
using SFA.DAS.Apprenticeships.Infrastructure.Extensions;

namespace SFA.DAS.Apprenticeships.Functions.AppStart;

internal static class NServiceBusConfiguration
{
    internal static IHostBuilder ConfigureNServiceBusForSubscribe(this IHostBuilder hostBuilder)
    {

        hostBuilder.UseNServiceBus((config, endpointConfiguration) =>
        {
            endpointConfiguration.AdvancedConfiguration.Conventions().SetConventions();

            var value = config["ApplicationSettings:NServiceBusLicense"];
            if (!string.IsNullOrEmpty(value))
            {
                var decodedLicence = WebUtility.HtmlDecode(value);
                endpointConfiguration.AdvancedConfiguration.License(decodedLicence);
            }

            CheckCreateQueues(config);
        });

        return hostBuilder;
    }

    /// <summary>
    /// Check if the queues exist and create them if they don't
    /// </summary>
    private static void CheckCreateQueues(IConfiguration configuration)
    {
        var queueTriggers = GetQueueTriggers();

        var connectionString = configuration["ApplicationSettings:NServiceBusConnectionString"];
        var fullyQualifiedNamespace = connectionString.GetFullyQualifiedNamespace();
        var adminClient = new ServiceBusAdministrationClient(fullyQualifiedNamespace, new DefaultAzureCredential());

        foreach (var queueTrigger in queueTriggers)
        {
            if (!adminClient.QueueExistsAsync(queueTrigger.QueueName).GetAwaiter().GetResult())
            {
                var errorQueue = $"{queueTrigger.QueueName}-error";

                adminClient.CreateQueueAsync(errorQueue).GetAwaiter().GetResult();

                var queue = new CreateQueueOptions(queueTrigger.QueueName)
                {
                    ForwardDeadLetteredMessagesTo = errorQueue,
                };

                adminClient.CreateQueueAsync(queue).GetAwaiter().GetResult();
            }
        }
    }

    private static IEnumerable<ServiceBusTriggerAttribute> GetQueueTriggers()
    {
        var allAssemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(x => x.GetName().FullName.Contains("SFA.DAS"));

        var queueTriggers = allAssemblies
            .SelectMany(assembly => assembly.GetTypes())
            .SelectMany(type => type.GetMethods())
            .SelectMany(method => method.GetParameters())
            .SelectMany(parameter => parameter.GetCustomAttributes(typeof(ServiceBusTriggerAttribute), false))
            .Cast<ServiceBusTriggerAttribute>();

        return queueTriggers;
    }
}

