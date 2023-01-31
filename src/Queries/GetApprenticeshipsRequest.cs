using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.Queries
{
    public class GetApprenticeshipsRequest : IQuery
    {
        public long Ukprn { get; }
        public FundingPlatform? FundingPlatform { get; set; }

        public GetApprenticeshipsRequest(long ukprn, FundingPlatform fundingPlatform)
        {
            Ukprn = ukprn;
            FundingPlatform = fundingPlatform;
        }
    }
}
