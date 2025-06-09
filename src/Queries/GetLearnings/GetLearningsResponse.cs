using SFA.DAS.Learning.DataTransferObjects;

namespace SFA.DAS.Learning.Queries.GetLearnings
{
    public class GetLearningsResponse
    {
        public IEnumerable<Apprenticeship> Apprenticeships { get; set; }

        public GetLearningsResponse(IEnumerable<Apprenticeship> apprenticeships)
        {
            Apprenticeships = apprenticeships;
        }
    }
}
