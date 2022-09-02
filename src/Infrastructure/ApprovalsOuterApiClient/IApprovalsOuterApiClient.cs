namespace SFA.DAS.Apprenticeships.Infrastructure.ApprovalsOuterApiClient;

public interface IApprovalsOuterApiClient
{
    Task<int> GetFundingBandMaximum(int courseCode);
}