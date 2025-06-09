using SFA.DAS.Learning.DataTransferObjects;

namespace SFA.DAS.Learning.Queries.GetPendingStartDateChange;

public class GetPendingStartDateChangeResponse
{
    public bool HasPendingStartDateChange { get; set; }
    public PendingStartDateChange PendingStartDateChange { get; set; } = null!;
}