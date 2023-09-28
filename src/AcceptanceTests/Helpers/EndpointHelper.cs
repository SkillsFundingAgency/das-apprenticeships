using NServiceBus;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;

namespace SFA.DAS.Apprenticeships.AcceptanceTests.Helpers;

public static class EndpointHelper  
{
    public static string EventStorageFolder => Path.Combine(Directory.GetCurrentDirectory()[..Directory.GetCurrentDirectory().IndexOf("src", StringComparison.Ordinal)], @"src\.learningtransport");

    public static async Task<IEndpointInstance> StartEndpoint(string endpointName, bool isSendOnly, Type[] types)
    {
        ClearEventStorage();

        var endpointConfiguration = new EndpointConfiguration(endpointName);
        endpointConfiguration.AssemblyScanner().ThrowExceptions = false;

        if(isSendOnly) endpointConfiguration.SendOnly();

        endpointConfiguration.UseNewtonsoftJsonSerializer();
        endpointConfiguration.Conventions().DefiningEventsAs(types.Contains);

        var transport = endpointConfiguration.UseTransport<LearningTransport>();
        transport.StorageDirectory(EventStorageFolder);
        transport.Routing();

        return await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);
    }

    private static void ClearEventStorage()
    {
        var di = new DirectoryInfo(EventStorageFolder);
        if (!di.Exists) return;

        foreach (var file in di.GetFiles())
        {
            file.Delete();
        }

        foreach (var dir in di.GetDirectories())
        {
            dir.Delete(true);
        }
    }
}