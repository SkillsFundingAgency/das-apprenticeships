using SFA.DAS.Apprenticeships.DataTransferObjects;

namespace SFA.DAS.Apprenticeships.Queries.GetApprenticeship;

public class GetApprenticeshipResponse
{
    public GetApprenticeshipResponse(long ukprn, ApprenticeshipWithEpisodes apprenticeship)
    {
        Ukprn = ukprn;
        Apprenticeship = apprenticeship;
    }

    public long Ukprn { get; set; }
    public ApprenticeshipWithEpisodes Apprenticeship { get; set; }
}

