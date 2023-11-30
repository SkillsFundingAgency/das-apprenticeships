using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.DataAccess.Repositories;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Factories;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.Command.AddPriceHistory
{
    public class CreateApprenticeshipPriceChangeCommandHandler : ICommandHandler<CreateApprenticeshipPriceChangeRequest>
    {
        private readonly IApprenticeshipRepository _apprenticeshipRepository;
        private readonly IApprenticeshipFactory _apprenticeshipFactory;

        public CreateApprenticeshipPriceChangeCommandHandler(IApprenticeshipRepository apprenticeshipRepository, IApprenticeshipFactory apprenticeshipFactory)
        {
            _apprenticeshipRepository = apprenticeshipRepository;
            _apprenticeshipFactory = apprenticeshipFactory;
        }

        public async Task Handle(CreateApprenticeshipPriceChangeRequest command,
            CancellationToken cancellationToken = default)
        {
            var apprenticeship = await _apprenticeshipRepository.Get(command.ApprenticeshipKey);
            var model = new ApprenticeshipDomainModel.Get(apprenticeship);
            apprenticeship.AddPriceHistory(command.TrainingPrice, command.AssessmentPrice, command.TotalPrice, command.EffectiveFromDate, DateTime.Now, PriceChangeRequestStatus.Created);
            await _apprenticeshipRepository.Update(apprenticeship);
        }
    }
}
