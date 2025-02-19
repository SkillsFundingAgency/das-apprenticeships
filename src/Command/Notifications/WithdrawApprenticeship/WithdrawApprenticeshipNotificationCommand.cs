using SFA.DAS.Apprenticeships.Command.WithdrawApprenticeship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Domain;

namespace SFA.DAS.Apprenticeships.Command.Notifications.WithdrawApprenticeship
{
    public class WithdrawApprenticeshipNotificationCommand : ICommand
    {
        public long UKPRN { get; set; }
        public long ApprovalsApprenticeshipId { get; set; }
        public string ApprenticeFirstName { get; set; }
        public string ApprenticeLastName { get; set; }
    }

    public class WithdrawApprenticeshipNotificationCommandHandler : ICommandHandler<WithdrawApprenticeshipNotificationCommand, Outcome>
    {
        public async Task<Outcome> Handle(WithdrawApprenticeshipNotificationCommand command,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            //todo
            //call commitments - inner? outer? to get provider name, employer name, apprentice name we have but if it comes back on the same call is it better to use the version from commitments?
            //[Route("/provider/{providerId}/apprentices/{apprenticeshipId}/details")] call the outer like this
            throw new NotImplementedException();
        }
    }
}
