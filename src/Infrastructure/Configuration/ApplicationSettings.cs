namespace SFA.DAS.Apprenticeships.Infrastructure.Configuration
{
    public class ApplicationSettings
    {
        public string AzureWebJobsStorage { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public string QueueName { get; set; }
        public string TopicPath { get; set; }
    }
}
