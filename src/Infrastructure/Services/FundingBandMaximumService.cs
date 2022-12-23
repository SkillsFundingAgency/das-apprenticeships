using System.Text;
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
        var builder = new StringBuilder();
        builder.AppendLine($"Getting standard for course code {courseCode}");
        var standard = await _apprenticeshipsOuterApiClient.GetStandard(courseCode);
        builder.AppendLine($"Found standard with {standard.ApprenticeshipFunding.Count} apprenticeship funding records");

        foreach (var getStandardFundingResponse in standard.ApprenticeshipFunding)
        {
            builder.AppendLine($"Apprenticeship funding found: EffectiveFrom: {getStandardFundingResponse.EffectiveFrom}, EffectiveTo: {getStandardFundingResponse.EffectiveTo}, MaxEmployerLevyCap: {getStandardFundingResponse.MaxEmployerLevyCap}");
        }

        builder.AppendLine($"Start Date used for query (if null will return null): {startDate}");
        if (startDate == null)
            return null;

        throw new Exception(builder.ToString()); //todo obviously do not merge this it's a hack to diagnose an issue with this story that only manifests on demo, need to fix logging properly as a next priority once this ticket is done

        return standard.ApprenticeshipFunding.SingleOrDefault(x =>
                x.EffectiveFrom <= startDate
                && (startDate <= x.EffectiveTo || x.EffectiveTo == null))?.MaxEmployerLevyCap;
    }
}