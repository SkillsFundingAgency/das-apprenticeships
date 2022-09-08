namespace SFA.DAS.Apprenticeships.Infrastructure.Adapters;

public interface IFundingBandMaximumApiAdapter
{
    Task<int> GetFundingBandMaximum(int courseCode);
}