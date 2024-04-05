using SFA.DAS.Apprenticeships.DataTransferObjects;

namespace SFA.DAS.Apprenticeships.Queries.GetPendingStartDateChange;

public class GetPendingStartDateChangeResponse
{
    public bool HasPendingStartDateChange { get; set; }
    public PendingStartDateChange PendingStartDateChange { get; set; }
}