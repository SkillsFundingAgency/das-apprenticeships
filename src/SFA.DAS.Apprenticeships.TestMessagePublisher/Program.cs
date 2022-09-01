using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.Infrastructure;
using SFA.DAS.Apprenticeships.TestMessagePublisher;

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("local.settings.json")
    .AddEnvironmentVariables()
    .Build();

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
    {
        NServiceBusHelper.Add(services, config);
    })
    .Build();

Console.WriteLine("Select an option...");
Console.WriteLine("1. ApprovalCreatedCommand");
Console.WriteLine("X. Exit");
var choice = Console.ReadLine();
if (choice == "1")
{
    await PublishCreatedCommand(host.Services);
}

async Task PublishCreatedCommand(IServiceProvider services)
{
    using IServiceScope serviceScope = services.CreateScope();
    IServiceProvider provider = serviceScope.ServiceProvider;
    var messagePublisher = provider.GetRequiredService<IMessageSession>();

    await messagePublisher.Send(new ApprovalCreatedCommand {FundingEmployerAccountId = 123436, Uln = "2135546", FundingType = FundingType.Levy, ApprovalsApprenticeshipId = 34254, UKPRN = 4536546, ActualStartDate = DateTime.Now, EmployerAccountId = 2344536, LegalEntityName = "Test", AgreedPrice = 122345m, PlannedEndDate = DateTime.Now.AddYears(1), TrainingCode = "ABC123"});

    Console.WriteLine("Message published.");
    Console.WriteLine("Press enter to quit");
    Console.ReadLine();
}