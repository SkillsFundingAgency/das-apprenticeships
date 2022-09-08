using SFA.DAS.Apprenticeships.Infrastructure.ApprenticeshipsOuterApiClient;

namespace SFA.DAS.Apprenticeships.Infrastructure.Adapters;

public class FundingBandMaximumApiAdapter : IFundingBandMaximumApiAdapter
{
    private readonly IApprenticeshipsOuterApiClient _apprenticeshipsOuterApiClient;

    public FundingBandMaximumApiAdapter(IApprenticeshipsOuterApiClient apprenticeshipsOuterApiClient)
    {
        _apprenticeshipsOuterApiClient = apprenticeshipsOuterApiClient;
    }

    public async Task<int> GetFundingBandMaximum(int courseCode)
    {
        var standard = await _apprenticeshipsOuterApiClient.GetStandard(courseCode);
        return standard.MaxFunding;
    }
}