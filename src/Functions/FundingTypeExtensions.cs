using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.Functions
{
    public static class FundingTypeExtensions
    {
        public static FundingType ToFundingType(this Approvals.EventHandlers.Messages.FundingType fundingType)
        {
            switch (fundingType)
            {
                case Approvals.EventHandlers.Messages.FundingType.NonLevy:
                    return FundingType.NonLevy;
                case Approvals.EventHandlers.Messages.FundingType.Transfer:
                    return FundingType.Transfer;
                default:
                    return FundingType.Levy;
            }
        }
    }
}
