using Microsoft.Extensions.Hosting;
using System;
using NServiceBus;
using System.Net;
using SFA.DAS.Apprenticeships.Infrastructure.Extensions;
using System.Security.Cryptography;

namespace SFA.DAS.Apprenticeships.Functions.AppStart;

internal static class NServiceBusConfiguration
{
    internal static IHostBuilder ConfigureNServiceBusForSubscribe(this IHostBuilder hostBuilder)
    {

        hostBuilder.UseNServiceBus((config, endpointConfiguration) =>
        {
            endpointConfiguration.LogDiagnostics();

            endpointConfiguration.Transport.SubscriptionRuleNamingConvention = AzureRuleNameShortener.Shorten;
            endpointConfiguration.AdvancedConfiguration.SendFailedMessagesTo($"{Constants.EndpointName}-error");
            endpointConfiguration.AdvancedConfiguration.Conventions().SetConventions();

            endpointConfiguration.UseSerialization<SystemJsonSerializer>();

            endpointConfiguration.AdvancedConfiguration.EnableInstallers();

            var value = config["ApplicationSettings:NServiceBusLicense"];
            if (!string.IsNullOrEmpty(value))
            {
                var decodedLicence = WebUtility.HtmlDecode(value);
                endpointConfiguration.AdvancedConfiguration.License(decodedLicence);
            }
        });

        return hostBuilder;
    }

    internal static class AzureRuleNameShortener
    {
        private const int AzureServiceBusRuleNameMaxLength = 50;

        public static string Shorten(Type type)
        {
            var ruleName = type.FullName;
            if (ruleName!.Length <= AzureServiceBusRuleNameMaxLength)
            {
                return ruleName;
            }

            var bytes = System.Text.Encoding.Default.GetBytes(ruleName);
            var hash = MD5.HashData(bytes);
            return new Guid(hash).ToString();
        }
    }
}
