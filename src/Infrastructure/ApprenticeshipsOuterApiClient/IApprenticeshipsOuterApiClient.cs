namespace SFA.DAS.Apprenticeships.Infrastructure.ApprenticeshipsOuterApiClient;

public interface IApprenticeshipsOuterApiClient
{
    Task<int> GetFundingBandMaximum(int courseCode);
}