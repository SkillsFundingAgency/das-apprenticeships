namespace SFA.DAS.Learning.Queries.GetApprenticeshipPrice
{
    public class GetApprenticeshipPriceRequest : IQuery
    {
        public Guid ApprenticeshipKey { get; set; }
    }
}
