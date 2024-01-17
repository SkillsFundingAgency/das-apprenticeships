namespace SFA.DAS.Apprenticeships.Queries.GetPendingPriceChange
{
    public class GetPendingPriceChangeRequest : IQuery
    {
        public GetPendingPriceChangeRequest(Guid apprenticeshipKey)
        {
            ApprenticeshipKey = apprenticeshipKey;
        }

        public Guid ApprenticeshipKey { get; set; }
    }
}
