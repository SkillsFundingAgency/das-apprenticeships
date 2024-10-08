using SFA.DAS.Apprenticeships.DataTransferObjects;

namespace SFA.DAS.Apprenticeships.Queries.GetApprenticeshipsWithEpisodes;

public class GetApprenticeshipsWithEpisodesResponse
{
    public GetApprenticeshipsWithEpisodesResponse(long ukprn, List<ApprenticeshipWithEpisodes> apprenticeships)
    {
        Ukprn = ukprn;
        Apprenticeships = apprenticeships;
    }

    public long Ukprn { get; set; }
    public List<ApprenticeshipWithEpisodes> Apprenticeships { get; set; }
}

