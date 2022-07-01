using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace SFA.DAS.Apprenticeships.Acceptance;

public interface IOrchestrationData
{
    DurableOrchestrationStatus Status { get; set; }
}