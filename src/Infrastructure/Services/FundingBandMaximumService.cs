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
        _logger.Log(LogLevel.Trace, $"Getting standard for course code {courseCode}");
        var standard = await _apprenticeshipsOuterApiClient.GetStandard(courseCode);
        _logger.Log(LogLevel.Trace, $"Found standard with {standard.ApprenticeshipFunding.Count} apprenticeship funding records");

        foreach (var getStandardFundingResponse in standard.ApprenticeshipFunding)
        {
            _logger.Log(LogLevel.Trace, $"Apprenticeship funding found: EffectiveFrom: {getStandardFundingResponse.EffectiveFrom}, EffectiveTo: {getStandardFundingResponse.EffectiveTo}, MaxEmployerLevyCap: {getStandardFundingResponse.MaxEmployerLevyCap}");
        }

        _logger.Log(LogLevel.Trace, $"Start Date used for query (if null will return null): {startDate}");
        if (startDate == null)
            return null;

        return standard.ApprenticeshipFunding.SingleOrDefault(x =>
                x.EffectiveFrom <= startDate
                && (startDate <= x.EffectiveTo || x.EffectiveTo == null))?.MaxEmployerLevyCap;
    }
}