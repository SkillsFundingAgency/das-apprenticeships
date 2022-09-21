using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace SFA.DAS.Apprenticeships.Infrastructure.ApprenticeshipsOuterApiClient;

[ExcludeFromCodeCoverage]
public class StandardSummary
{
    public string StandardUId { get; set; }
    [JsonProperty("maxFunding")]
    public int FundingBandMaximum { get; set; }
    public int LarsCode { get; set; }
    public int VersionMajor { get; set; }
    public int VersionMinor { get; set; }
    public string Status { get; set; }
    public bool IsLatestVersion { get; set; }
}