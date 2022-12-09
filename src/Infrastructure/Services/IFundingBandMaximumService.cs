namespace SFA.DAS.Apprenticeships.Infrastructure.Services;

public interface IFundingBandMaximumService
{
    Task<int?> GetFundingBandMaximum(int courseCode, DateTime? startDate);
}