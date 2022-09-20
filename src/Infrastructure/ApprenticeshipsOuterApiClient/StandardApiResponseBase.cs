using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace SFA.DAS.Apprenticeships.Infrastructure.ApprenticeshipsOuterApiClient;

[ExcludeFromCodeCoverage]
public abstract class StandardApiResponseBase
{
    public StandardDate StandardDates { get; set; }
    public List<ApprenticeshipFunding> ApprenticeshipFunding { get; set; }

    [JsonIgnore]
    public int MaxFunding => GetFundingDetails(nameof(MaxFunding));
    [JsonIgnore]
    public int TypicalDuration => GetFundingDetails(nameof(TypicalDuration));
    [JsonIgnore]
    public bool IsActive => this.IsStandardActive();
    public int MaxFundingOn(DateTime effectiveDate) => GetFundingDetails(nameof(MaxFunding), effectiveDate);
    protected virtual int GetFundingDetails(string prop, DateTime? effectiveDate = null) => this.FundingDetails(prop, effectiveDate);
}