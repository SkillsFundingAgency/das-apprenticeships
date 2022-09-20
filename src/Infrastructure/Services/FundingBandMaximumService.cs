using SFA.DAS.Apprenticeships.Infrastructure.ApprenticeshipsOuterApiClient;

namespace SFA.DAS.Apprenticeships.Infrastructure.Services;

public class FundingBandMaximumService : IFundingBandMaximumService
{
    private readonly IApprenticeshipsOuterApiClient _apprenticeshipsOuterApiClient;

    public FundingBandMaximumService(IApprenticeshipsOuterApiClient apprenticeshipsOuterApiClient)
    {
        _apprenticeshipsOuterApiClient = apprenticeshipsOuterApiClient;
    }

    public async Task<int?> GetFundingBandMaximum(int courseCode, DateTime? actualStartDate)
    {
        var standard = await _apprenticeshipsOuterApiClient.GetStandard(courseCode);
        if (actualStartDate == null)
            return standard.MaxFunding;

        return standard.ApprenticeshipFunding.SingleOrDefault(x =>
                x.EffectiveFrom <= actualStartDate
                && (actualStartDate <= x.EffectiveTo || x.EffectiveTo == null))?.MaxEmployerLevyCap;
    }
}