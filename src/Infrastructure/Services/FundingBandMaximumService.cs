using Microsoft.Extensions.Logging;
using SFA.DAS.Apprenticeships.Infrastructure.ApprenticeshipsOuterApiClient;

namespace SFA.DAS.Apprenticeships.Infrastructure.Services;

public class FundingBandMaximumService : IFundingBandMaximumService
{
    private readonly IApprenticeshipsOuterApiClient _apprenticeshipsOuterApiClient;
    private readonly ILogger<FundingBandMaximumService> _logger;

    public FundingBandMaximumService(IApprenticeshipsOuterApiClient apprenticeshipsOuterApiClient, ILogger<FundingBandMaximumService> logger)
    {
        _apprenticeshipsOuterApiClient = apprenticeshipsOuterApiClient;
        _logger = logger;
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