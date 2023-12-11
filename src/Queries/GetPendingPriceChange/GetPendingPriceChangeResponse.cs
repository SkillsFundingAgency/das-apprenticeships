using SFA.DAS.Apprenticeships.DataTransferObjects;

namespace SFA.DAS.Apprenticeships.Queries.GetPendingPriceChange
{
    public class GetPendingPriceChangeResponse
    {
        public bool HasPendingPriceChange { get; set; }
        public ApprenticeshipPrice PendingPriceChange { get; set; }
    }
}