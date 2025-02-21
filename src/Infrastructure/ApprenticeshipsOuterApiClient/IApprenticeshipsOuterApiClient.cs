using SFA.DAS.Apprenticeships.Infrastructure.ApprenticeshipsOuterApiClient.Calendar;
using SFA.DAS.Apprenticeships.Infrastructure.ApprenticeshipsOuterApiClient.Standards;

namespace SFA.DAS.Apprenticeships.Infrastructure.ApprenticeshipsOuterApiClient;

public interface IApprenticeshipsOuterApiClient
{
    Task<GetStandardResponse> GetStandard(int courseCode);
    Task<GetAcademicYearsResponse> GetAcademicYear(DateTime searchYear);
    Task HandleWithdrawalNotifications(Guid apprenticeshipKey, HandleWithdrawalNotificationsRequest request);
}