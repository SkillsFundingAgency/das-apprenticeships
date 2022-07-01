using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace SFA.DAS.Apprenticeships.Acceptance;

public class OrchestrationData : IOrchestrationData
{
    public DurableOrchestrationStatus Status { get; set; }
}

//todo I don't think this is needed
public class TestDurableOrchestrationContext : IDurableOrchestrationContext
{
    public TInput GetInput<TInput>()
    {
        throw new NotImplementedException();
    }

    public void SetOutput(object output)
    {
        throw new NotImplementedException();
    }

    public void ContinueAsNew(object input, bool preserveUnprocessedEvents = false)
    {
        throw new NotImplementedException();
    }

    public void SetCustomStatus(object customStatusObject)
    {
        throw new NotImplementedException();
    }

    public Task<DurableHttpResponse> CallHttpAsync(HttpMethod method, Uri uri, string content = null, HttpRetryOptions retryOptions = null)
    {
        throw new NotImplementedException();
    }

    public Task<DurableHttpResponse> CallHttpAsync(DurableHttpRequest req)
    {
        throw new NotImplementedException();
    }

    public Task<TResult> CallEntityAsync<TResult>(EntityId entityId, string operationName)
    {
        throw new NotImplementedException();
    }

    public Task CallEntityAsync(EntityId entityId, string operationName)
    {
        throw new NotImplementedException();
    }

    public Task<TResult> CallEntityAsync<TResult>(EntityId entityId, string operationName, object operationInput)
    {
        throw new NotImplementedException();
    }

    public Task CallEntityAsync(EntityId entityId, string operationName, object operationInput)
    {
        throw new NotImplementedException();
    }

    public Task<TResult> CallSubOrchestratorAsync<TResult>(string functionName, object input)
    {
        throw new NotImplementedException();
    }

    public Task<TResult> CallSubOrchestratorAsync<TResult>(string functionName, string instanceId, object input)
    {
        throw new NotImplementedException();
    }

    public Task CallSubOrchestratorAsync(string functionName, object input)
    {
        throw new NotImplementedException();
    }

    public Task CallSubOrchestratorAsync(string functionName, string instanceId, object input)
    {
        throw new NotImplementedException();
    }

    public Task<TResult> CallSubOrchestratorWithRetryAsync<TResult>(string functionName, RetryOptions retryOptions, string instanceId,
        object input)
    {
        throw new NotImplementedException();
    }

    public Task CallSubOrchestratorWithRetryAsync(string functionName, RetryOptions retryOptions, object input)
    {
        throw new NotImplementedException();
    }

    public Task CallSubOrchestratorWithRetryAsync(string functionName, RetryOptions retryOptions, string instanceId, object input)
    {
        throw new NotImplementedException();
    }

    public Task<TResult> CallSubOrchestratorWithRetryAsync<TResult>(string functionName, RetryOptions retryOptions, object input)
    {
        throw new NotImplementedException();
    }

    public Task<T> CreateTimer<T>(DateTime fireAt, T state, CancellationToken cancelToken)
    {
        throw new NotImplementedException();
    }

    public Task CreateTimer(DateTime fireAt, CancellationToken cancelToken)
    {
        throw new NotImplementedException();
    }

    public Task<T> WaitForExternalEvent<T>(string name)
    {
        throw new NotImplementedException();
    }

    public Task WaitForExternalEvent(string name)
    {
        throw new NotImplementedException();
    }

    public Task WaitForExternalEvent(string name, TimeSpan timeout, CancellationToken cancelToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task<T> WaitForExternalEvent<T>(string name, TimeSpan timeout, CancellationToken cancelToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task<T> WaitForExternalEvent<T>(string name, TimeSpan timeout, T defaultValue,
        CancellationToken cancelToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task<IDisposable> LockAsync(params EntityId[] entities)
    {
        throw new NotImplementedException();
    }

    public bool IsLocked(out IReadOnlyList<EntityId> ownedLocks)
    {
        throw new NotImplementedException();
    }

    public Guid NewGuid()
    {
        throw new NotImplementedException();
    }

    public Task<TResult> CallActivityAsync<TResult>(string functionName, object input)
    {
        throw new NotImplementedException();
    }

    public Task CallActivityAsync(string functionName, object input)
    {
        throw new NotImplementedException();
    }

    public Task<TResult> CallActivityWithRetryAsync<TResult>(string functionName, RetryOptions retryOptions, object input)
    {
        throw new NotImplementedException();
    }

    public Task CallActivityWithRetryAsync(string functionName, RetryOptions retryOptions, object input)
    {
        throw new NotImplementedException();
    }

    public void SignalEntity(EntityId entity, string operationName, object operationInput = null)
    {
        throw new NotImplementedException();
    }

    public void SignalEntity(EntityId entity, DateTime scheduledTimeUtc, string operationName, object operationInput = null)
    {
        throw new NotImplementedException();
    }

    public string StartNewOrchestration(string functionName, object input, string instanceId = null)
    {
        throw new NotImplementedException();
    }

    public TEntityInterface CreateEntityProxy<TEntityInterface>(string entityKey)
    {
        throw new NotImplementedException();
    }

    public TEntityInterface CreateEntityProxy<TEntityInterface>(EntityId entityId)
    {
        throw new NotImplementedException();
    }

    public string Name { get; }
    public string InstanceId { get; }
    public string ParentInstanceId { get; }
    public DateTime CurrentUtcDateTime { get; }
    public bool IsReplaying { get; }
}