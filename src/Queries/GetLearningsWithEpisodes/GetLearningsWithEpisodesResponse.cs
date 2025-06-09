using SFA.DAS.Learning.DataTransferObjects;

namespace SFA.DAS.Learning.Queries.GetLearningsWithEpisodes;

public class GetLearningsWithEpisodesResponse
{
    public GetLearningsWithEpisodesResponse(long ukprn, List<ApprenticeshipWithEpisodes> apprenticeships)
    {
        Ukprn = ukprn;
        Apprenticeships = apprenticeships;
    }

    public long Ukprn { get; set; }
    public List<ApprenticeshipWithEpisodes> Apprenticeships { get; set; }
}

