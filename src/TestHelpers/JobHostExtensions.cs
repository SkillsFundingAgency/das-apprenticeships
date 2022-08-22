using DurableTask.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace SFA.DAS.Apprenticeships.TestHelpers;

public static class JobHostExtensions
{
    public static async Task<IJobHost> WaitFor(this IJobHost jobs, string orchestration, TimeSpan? timeout = null, string expectedCustomStatus = null)
    {
        await jobs.CallAsync(nameof(WaitForFunction), new Dictionary<string, object>
        {
            ["timeout"] = timeout,
            ["name"] = orchestration,
            ["expectedCustomStatus"] = expectedCustomStatus
        });

        return jobs;
    }

    public static async Task<IJobHost> WaitFor(this Task<IJobHost> task, string orchestration, TimeSpan? timeout = null)
    {
        var jobs = await task;
        return await jobs.WaitFor(orchestration, timeout);
    }

    public static async Task<IJobHost> ThrowIfFailed(this Task<IJobHost> task)
    {
        var jobs = await task;
        await jobs.CallAsync(nameof(ThrowIfFailedFunction));

        return jobs;
    }

    public static async Task<IJobHost> Purge(this Task<IJobHost> task)
    {
        var jobs = await task;
        await jobs.CallAsync(nameof(PurgeFunction));

        return jobs;
    }

    public static async Task<IJobHost> Purge(this IJobHost jobs)
    {
        await jobs.CallAsync(nameof(PurgeFunction));
        return jobs;
    }

    public static async Task<IJobHost> Terminate(this IJobHost jobs)
    {
        await jobs.CallAsync(nameof(TerminateFunction));
        return jobs;
    }
}

public static class PurgeFunction
{
    [FunctionName(nameof(PurgeFunction))]
    public static async Task Run([DurableClient] IDurableOrchestrationClient client)
    {
        await client.PurgeInstanceHistoryAsync(
            DateTime.MinValue,
            null,
            new[] {
                OrchestrationStatus.Completed,
                OrchestrationStatus.Terminated,
                OrchestrationStatus.Failed,
            });
    }
}

public static class TerminateFunction
{
    [FunctionName(nameof(TerminateFunction))]
    public static async Task Run([DurableClient] IDurableOrchestrationClient client)
    {
        var all = await client.ListInstancesAsync(new OrchestrationStatusQueryCondition
        {
            RuntimeStatus = new[] { OrchestrationRuntimeStatus.Pending, OrchestrationRuntimeStatus.Running, OrchestrationRuntimeStatus.ContinuedAsNew }
        }, CancellationToken.None);

        await Task.WhenAll(all.DurableOrchestrationState.Select(async o => await client.TerminateAsync(o.InstanceId, "Clean up test data.")));
    }
}

public class OrchestrationStarterInfo
{
    public string StarterName { get; private set; }
    public Dictionary<string, object> StarterArgs { get; private set; }
    public string OrchestrationName { get; private set; }
    public TimeSpan? Timeout { get; private set; }
    public string ExpectedCustomStatus { get; private set; }

    public OrchestrationStarterInfo(
        string starterName,
        string orchestrationName,
        Dictionary<string, object> args = null,
        TimeSpan? timeout = null,
        string expectedCustomStatus = null)
    {
        if (string.IsNullOrEmpty(starterName)) throw new ArgumentException("Missing starter name");
        if (string.IsNullOrEmpty(orchestrationName)) throw new ArgumentException("Missing starter name");

        StarterName = starterName;
        OrchestrationName = orchestrationName;
        if (args == null)
        {
            args = new Dictionary<string, object>();
        }
        if (timeout == null)
        {
            timeout = new TimeSpan(0, 60, 0);
        }
        Timeout = timeout;
        StarterArgs = args;
        ExpectedCustomStatus = expectedCustomStatus;
    }
}

public static class WaitForFunction
{
    [FunctionName(nameof(WaitForFunction))]
    [NoAutomaticTrigger]
    public static async Task Run([DurableClient] IDurableOrchestrationClient client, string name, TimeSpan? timeout, string expectedCustomStatus)
    {
        using var cts = new CancellationTokenSource();
        if (timeout != null)
        {
            cts.CancelAfter(timeout.Value);
        }

        await client.Wait(status => status.All(x => OrchestrationsCompleteOrAwaitingInput(name, expectedCustomStatus, x)), cts.Token);
    }

    private static bool OrchestrationsCompleteOrAwaitingInput(string orchestratorName, string expectedCustomStatus, DurableOrchestrationStatus orchestrationStatus)
    {
        var customStatus = orchestrationStatus.CustomStatus.ToObject<string>();
        return orchestrationStatus.Name != orchestratorName || (expectedCustomStatus != null && customStatus == expectedCustomStatus);
    }
}

internal static class DurableOrchestrationClientExtensions
{
    public static async Task Wait(this IDurableOrchestrationClient client,
        Func<IEnumerable<DurableOrchestrationStatus>, bool> until, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var instances = await client.ListInstancesAsync(new OrchestrationStatusQueryCondition
            {
                RuntimeStatus = new[] { OrchestrationRuntimeStatus.Pending, OrchestrationRuntimeStatus.Running, OrchestrationRuntimeStatus.ContinuedAsNew },
                TaskHubNames = new[] { client.TaskHubName }
            }, token);

            if (until(instances.DurableOrchestrationState))
            {
                break;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(100), token);
        }
    }
}

public static class ThrowIfFailedFunction
{
    [FunctionName(nameof(ThrowIfFailedFunction))]
    public static async Task Run([DurableClient] IDurableOrchestrationClient client)
    {
        var failed = await client.ListInstancesAsync(new OrchestrationStatusQueryCondition { TaskHubNames = new[] { client.TaskHubName }, RuntimeStatus = new[] { OrchestrationRuntimeStatus.Failed } }, CancellationToken.None);
        if (failed.DurableOrchestrationState.Any())
        {
            throw new AggregateException(failed.DurableOrchestrationState.Select(x => new Exception(x.Output.ToString())));
        }
    }
}

public class EndpointInfo
{
    public string StarterName { get; private set; }
    public Dictionary<string, object> StarterArgs { get; private set; }

    public EndpointInfo(
        string starterName,
        Dictionary<string, object> args = null)
    {
        if (string.IsNullOrEmpty(starterName)) throw new ArgumentException("Missing starter name");

        StarterName = starterName;
        if (args == null)
        {
            args = new Dictionary<string, object>();
        }
        StarterArgs = args;
    }
}

public class GetStatusFunction
{
    private readonly IOrchestrationData _orchestrationData;

    public GetStatusFunction(IOrchestrationData orchestrationData)
    {
        _orchestrationData = orchestrationData;
    }

    [FunctionName(nameof(GetStatusFunction))]
    public async Task Run([DurableClient] IDurableOrchestrationClient client, string instanceId)
    {
        _orchestrationData.Status = await client.GetStatusAsync(instanceId);
    }
}

public interface IOrchestrationData
{
    DurableOrchestrationStatus Status { get; set; }
}

public class OrchestrationData : IOrchestrationData
{
    public DurableOrchestrationStatus Status { get; set; }
}