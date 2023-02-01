using SFA.DAS.Apprenticeships.DataTransferObjects;

namespace SFA.DAS.Apprenticeships.Queries
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
