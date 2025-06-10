using SFA.DAS.Learning.Enums;

namespace SFA.DAS.Learning.Queries.GetLearnings;

public class GetLearningsRequest : IQuery
{
    public long Ukprn { get; }
    public FundingPlatform? FundingPlatform { get; set; }

    public GetLearningsRequest(long ukprn, FundingPlatform? fundingPlatform)
    {
        Ukprn = ukprn;
        FundingPlatform = fundingPlatform;
    }
}
