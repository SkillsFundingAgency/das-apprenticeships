namespace SFA.DAS.Apprenticeships.Queries.GetApprenticeshipPrice
{
    public class GetApprenticeshipPriceRequest : IQuery
    {
        public Guid ApprenticeshipKey { get; set; }
    }
}
