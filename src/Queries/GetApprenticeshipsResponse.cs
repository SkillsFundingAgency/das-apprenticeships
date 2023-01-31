namespace SFA.DAS.Apprenticeships.Queries
{
    public class GetApprenticeshipsResponse
    {
        public IEnumerable<DataTransferObjects.Apprenticeship> Apprenticeships { get; set; }

        public GetApprenticeshipsResponse(IEnumerable<DataTransferObjects.Apprenticeship> apprenticeships)
        {
            Apprenticeships = apprenticeships;
        }
    }
}
