using SFA.DAS.Apprenticeships.DataTransferObjects;

namespace SFA.DAS.Apprenticeships.Queries.GetPendingPriceChange
{
    public class GetPendingPriceChangeResponse
    {
        public bool HasPendingPriceChange { get; set; }
        public PendingPriceChange PendingPriceChange { get; set; }
    }
}