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

    }

    public class WithdrawApprenticeshipNotificationCommandHandler : ICommandHandler<WithdrawApprenticeshipNotificationCommand, Outcome>
    {
        public async Task<Outcome> Handle(WithdrawApprenticeshipNotificationCommand command,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}
