using SFA.DAS.Apprenticeships.Command.Notifications.WithdrawApprenticeship;
using SFA.DAS.Apprenticeships.Command.WithdrawApprenticeship;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;

namespace SFA.DAS.Apprenticeships.Command.Notifications
{
    public static class CommandExtensions
    {
        public static WithdrawApprenticeshipNotificationCommand ToNotificationCommand(this WithdrawApprenticeshipCommand command, ApprenticeshipDomainModel apprenticeship)
        {
            return new WithdrawApprenticeshipNotificationCommand
            {
                UKPRN = command.UKPRN,
                ApprovalsApprenticeshipId = apprenticeship.ApprovalsApprenticeshipId,
                ApprenticeFirstName = apprenticeship.FirstName,
                ApprenticeLastName = apprenticeship.LastName
            };
        }
    }
}
