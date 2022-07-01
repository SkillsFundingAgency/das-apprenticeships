using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;

namespace SFA.DAS.Apprenticeships.Infrastructure
{
    public static class QueueHelper
    {
        public static async Task EnsureTopic(string serviceBusConnectionString, string topicPath)
        {
            if(serviceBusConnectionString.Equals("UseLearningEndpoint=true", StringComparison.CurrentCultureIgnoreCase))
                return;
            
            var manageClient = new ManagementClient(serviceBusConnectionString);

            if (await manageClient.TopicExistsAsync(topicPath))
                return;

            await manageClient.CreateTopicAsync(new TopicDescription(topicPath));
        }

        public static async Task EnsureQueue(string serviceBusConnectionString, string queueName)
        {
            if (serviceBusConnectionString.Equals("UseLearningEndpoint=true", StringComparison.CurrentCultureIgnoreCase))
                return;

            var manageClient = new ManagementClient(serviceBusConnectionString);

            if (await manageClient.QueueExistsAsync(queueName))
                return;

            var queueDescription = new QueueDescription(queueName)
            {
                DefaultMessageTimeToLive = TimeSpan.FromDays(7),
                EnableDeadLetteringOnMessageExpiration = true,
                LockDuration = TimeSpan.FromMinutes(5),
                MaxDeliveryCount = 50,
                MaxSizeInMB = 5120,
                Path = queueName
            };

            await manageClient.CreateQueueAsync(queueDescription);
        }

        public static async Task EnsureSubscription(string serviceBusConnectionString, string topicPath, string queueName, Type eventType)
        {
            if (serviceBusConnectionString.Equals("UseLearningEndpoint=true", StringComparison.CurrentCultureIgnoreCase))
                return;

            _ = await GetOrCreateSubscription(serviceBusConnectionString, topicPath, queueName, CancellationToken.None);

            var existingRules = await GetExistingRules(serviceBusConnectionString, topicPath, queueName, CancellationToken.None);

            if (!existingRules.Any(x => x.Name == eventType.Name))
            {
                CreateNewSubscriptionRule(serviceBusConnectionString, topicPath, eventType, queueName, CancellationToken.None);
            }
        }

        private static void CreateNewSubscriptionRule(string serviceBusConnectionString, string topicPath, Type type, string endpointName, CancellationToken cancellationToken)
        {
            var manageClient = new ManagementClient(serviceBusConnectionString);
            var ruleDescription = new RuleDescription
            {
                Filter = new SqlFilter($"[NServiceBus.EnclosedMessageTypes] LIKE '%{type.FullName}%'"),
                Name = type.Name
            };

            manageClient.CreateRuleAsync(topicPath, endpointName, ruleDescription, cancellationToken);
        }

        private static async Task<IList<RuleDescription>> GetExistingRules(string serviceBusConnectionString, string topicPath, string subscriptionName, CancellationToken cancellationToken)
        {
            var manageClient = new ManagementClient(serviceBusConnectionString);
            return await manageClient.GetRulesAsync(topicPath, subscriptionName, cancellationToken: cancellationToken);
        }

        private static async Task<SubscriptionDescription> GetOrCreateSubscription(string serviceBusConnectionString, string topicPath, string endpointName, CancellationToken cancellationToken)
        {
            var manageClient = new ManagementClient(serviceBusConnectionString);

            SubscriptionDescription subscriptionDescription;
            if (!await manageClient.SubscriptionExistsAsync(topicPath, endpointName, cancellationToken))
            {
                subscriptionDescription = new SubscriptionDescription(topicPath, endpointName)
                {
                    ForwardTo = endpointName,
                    UserMetadata = endpointName,
                    EnableBatchedOperations = true,
                    MaxDeliveryCount = Int32.MaxValue,
                    EnableDeadLetteringOnFilterEvaluationExceptions = false,
                    LockDuration = TimeSpan.FromMinutes(5)
                };
                var defaultRule = new RuleDescription("$default") { Filter = new SqlFilter("1=0") };
                await manageClient.CreateSubscriptionAsync(
                   subscriptionDescription, defaultRule, cancellationToken);
            }
            else
            {
                subscriptionDescription =
                    await manageClient.GetSubscriptionAsync(topicPath, endpointName, cancellationToken);
            }

            return subscriptionDescription;
        }
    }
}