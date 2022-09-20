using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Apprenticeships.Infrastructure.Configuration;

[ExcludeFromCodeCoverage]
public class ApprenticeshipsOuterApiConfiguration
{
    public string Key { get; set; }
    public string BaseUrl { get; set; }
}