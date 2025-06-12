namespace SFA.DAS.Learning.Queries.GetLearningPrice
{
    public class GetLearningPriceRequest : IQuery
    {
        public Guid ApprenticeshipKey { get; set; }
    }
}
