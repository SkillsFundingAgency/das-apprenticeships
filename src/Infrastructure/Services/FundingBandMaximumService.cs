using SFA.DAS.Apprenticeships.Infrastructure.ApprenticeshipsOuterApiClient;

namespace SFA.DAS.Apprenticeships.Infrastructure.Services;

public class FundingBandMaximumService : IFundingBandMaximumService
{
    private readonly IApprenticeshipsOuterApiClient _apprenticeshipsOuterApiClient;

    public FundingBandMaximumService(IApprenticeshipsOuterApiClient apprenticeshipsOuterApiClient)
    {
        _apprenticeshipsOuterApiClient = apprenticeshipsOuterApiClient;
    }

    public async Task<int?> GetFundingBandMaximum(int courseCode, DateTime? startDate)
    {
        var standard = await _apprenticeshipsOuterApiClient.GetStandard(courseCode);
        if (startDate == null)
            return null;

        return standard.ApprenticeshipFunding.SingleOrDefault(x =>
                x.EffectiveFrom <= startDate
                && (startDate <= x.EffectiveTo || x.EffectiveTo == null))?.MaxEmployerLevyCap;
    }
}