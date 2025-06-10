namespace SFA.DAS.Learning.Queries.GetLearnings
{
    public class GetLearningsResponse(IEnumerable<DataTransferObjects.Learning> learnings)
    {
        public IEnumerable<DataTransferObjects.Learning> Learnings { get; set; } = learnings;
    }
}
