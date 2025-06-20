using System.ComponentModel;

namespace SFA.DAS.Apprenticeship.Types;

public enum FundingPlatform
{
    [Description("DASFundingPlatform")]
    DAS = 1,
    
    [Description("SLDFundingPlatform")]
    SLD = 2
}