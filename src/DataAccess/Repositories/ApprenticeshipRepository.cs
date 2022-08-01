using Microsoft.EntityFrameworkCore;
using SFA.DAS.Apprenticeships.DataAccess.Mappers.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Factories;
using SFA.DAS.Apprenticeships.Domain.Repositories;

namespace SFA.DAS.Apprenticeships.DataAccess.Repositories
{
    public class ApprenticeshipRepository : IApprenticeshipRepository
    {
        private readonly Lazy<ApprenticeshipsDataContext> _lazyContext;
        private readonly IDomainEventDispatcher _domainEventDispatcher;
        private readonly IApprenticeshipFactory _apprenticeshipFactory;
        private ApprenticeshipsDataContext DbContext => _lazyContext.Value;

        public ApprenticeshipRepository(Lazy<ApprenticeshipsDataContext> dbContext, IDomainEventDispatcher domainEventDispatcher, IApprenticeshipFactory apprenticeshipFactory)
        {
            _lazyContext = dbContext;
            _domainEventDispatcher = domainEventDispatcher;
            _apprenticeshipFactory = apprenticeshipFactory;
        }

        public async Task Add(Apprenticeship apprenticeship)
        {
            var apprenticeshipDataModel = apprenticeship.GetModel().Map();
            await DbContext.AddAsync(apprenticeshipDataModel);

            foreach (dynamic domainEvent in apprenticeship.FlushEvents())
            {
                await _domainEventDispatcher.Send(domainEvent);
            }
        }

        public async Task<Apprenticeship> Get(Guid key)
        {
            var apprenticeshipDataModel = await DbContext.Apprenticeships.Include(x => x.Approvals).SingleAsync(x => x.Key == key);
            var domainModel = apprenticeshipDataModel.Map();
            var domainObject = _apprenticeshipFactory.GetExisting(domainModel);
            return domainObject;
        }
    }
}
