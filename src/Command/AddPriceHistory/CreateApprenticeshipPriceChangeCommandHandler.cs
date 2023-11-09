using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Domain.Repositories;

namespace SFA.DAS.Apprenticeships.Command.AddPriceHistory
{
    internal class CreateApprenticeshipPriceChangeCommandHandler : ICommandHandler<CreateApprenticeshipPriceChangeRequest>
    {
        private readonly IApprenticeshipRepository _apprenticeshipRepository;

        public CreateApprenticeshipPriceChangeCommandHandler(IApprenticeshipRepository apprenticeshipRepository)
        {
            _apprenticeshipRepository = apprenticeshipRepository;
        }

        public Task Handle(CreateApprenticeshipPriceChangeRequest command,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            //_apprenticeshipRepository.Get(command.ApprenticeshipId); //todo we need the key!
            throw new NotImplementedException("todo we need the key!");
        }
    }
}
