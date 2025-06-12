using SFA.DAS.Learning.DataTransferObjects;

namespace SFA.DAS.Learning.Queries.GetPendingPriceChange
{
    public class GetPendingPriceChangeResponse
    {
        public bool HasPendingPriceChange { get; set; }
        public PendingPriceChange PendingPriceChange { get; set; }
    }
}