using SFA.DAS.Learning.Enums;

namespace SFA.DAS.Learning.Queries.GetApprenticeships;

public class GetApprenticeshipsRequest : IQuery
{
    public long Ukprn { get; }
    public FundingPlatform? FundingPlatform { get; set; }

    public GetApprenticeshipsRequest(long ukprn, FundingPlatform? fundingPlatform)
    {
        Ukprn = ukprn;
        FundingPlatform = fundingPlatform;
    }
}
