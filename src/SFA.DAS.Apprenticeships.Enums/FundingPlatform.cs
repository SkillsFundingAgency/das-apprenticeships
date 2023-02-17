using System.ComponentModel;

namespace SFA.DAS.Apprenticeships.Enums
{
    public enum FundingPlatform
    {
        [Description("DASFundingPlatform")]
        DAS = 1,
        
        [Description("SLDFundingPlatform")]
        SLD = 2
    }
}