using SFA.DAS.Apprenticeships.Infrastructure.ApprenticeshipsOuterApiClient;

namespace SFA.DAS.Apprenticeships.Infrastructure.Services;

public class FundingBandMaximumService : IFundingBandMaximumService
{
    private readonly IApprenticeshipsOuterApiClient _apprenticeshipsOuterApiClient;

    public FundingBandMaximumService(IApprenticeshipsOuterApiClient apprenticeshipsOuterApiClient)
    {
        _apprenticeshipsOuterApiClient = apprenticeshipsOuterApiClient;
    }

    public async Task<int> GetFundingBandMaximum(int courseCode)
    {
        var standard = await _apprenticeshipsOuterApiClient.GetStandard(courseCode);
        return standard.MaxFunding;
    }
}