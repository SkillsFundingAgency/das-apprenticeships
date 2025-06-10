namespace SFA.DAS.Learning.Infrastructure.Services;

public interface IFundingBandMaximumService
{
    Task<int?> GetFundingBandMaximum(int courseCode, DateTime? startDate);
}