namespace SFA.DAS.Apprenticeships.Infrastructure.ApprenticeshipsOuterApiClient;

public interface IApprenticeshipsOuterApiClient
{
    Task<GetStandardResponse> GetStandard(int courseCode);
}