using SFA.DAS.Apprenticeships.Domain.Factories;
using SFA.DAS.Apprenticeships.Domain.Repositories;

namespace SFA.DAS.Apprenticeships.Command.AddApproval
{
    public class AddApprovalCommandHandler : ICommandHandler<AddApprovalCommand>
    {
        private readonly IApprenticeshipFactory _apprenticeshipFactory;
        private readonly IApprenticeshipRepository _apprenticeshipRepository;

        public AddApprovalCommandHandler(IApprenticeshipFactory apprenticeshipFactory, IApprenticeshipRepository apprenticeshipRepository)
        {
            _apprenticeshipFactory = apprenticeshipFactory;
            _apprenticeshipRepository = apprenticeshipRepository;
        }

        public async Task Handle(AddApprovalCommand command, CancellationToken cancellationToken = default)
        {
            var apprenticeship = _apprenticeshipFactory.CreateNew();
            await _apprenticeshipRepository.Add(apprenticeship);
        }
    }
}
