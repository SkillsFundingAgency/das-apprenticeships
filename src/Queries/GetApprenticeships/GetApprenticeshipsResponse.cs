using SFA.DAS.Learning.DataTransferObjects;

namespace SFA.DAS.Learning.Queries.GetApprenticeships
{
    public class GetApprenticeshipsResponse
    {
        public IEnumerable<Apprenticeship> Apprenticeships { get; set; }

        public GetApprenticeshipsResponse(IEnumerable<Apprenticeship> apprenticeships)
        {
            Apprenticeships = apprenticeships;
        }
    }
}
