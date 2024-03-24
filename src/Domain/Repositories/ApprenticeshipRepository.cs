using Microsoft.EntityFrameworkCore;
using SFA.DAS.Apprenticeships.DataAccess;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Factories;

namespace SFA.DAS.Apprenticeships.Domain.Repositories
{
    public class ApprenticeshipRepository : IApprenticeshipRepository
    {
        private readonly Lazy<ApprenticeshipsDataContext> _lazyContext;
        private IDomainEventDispatcher _domainEventDispatcher;
        private readonly IApprenticeshipFactory _apprenticeshipFactory;
        private readonly IAccountIdValidator _accountIdValidator;
        private ApprenticeshipsDataContext DbContext => _lazyContext.Value;

        public ApprenticeshipRepository(Lazy<ApprenticeshipsDataContext> dbContext, IDomainEventDispatcher domainEventDispatcher, IApprenticeshipFactory apprenticeshipFactory, IAccountIdValidator accountIdValidator)
        {
            _lazyContext = dbContext;
            _domainEventDispatcher = domainEventDispatcher;
            _apprenticeshipFactory = apprenticeshipFactory;
            _accountIdValidator = accountIdValidator;
        }

        public async Task Add(ApprenticeshipDomainModel apprenticeship)
        {
            _accountIdValidator.ValidateAccountId(apprenticeship);
            await DbContext.AddAsync(apprenticeship.GetEntity());
            await DbContext.SaveChangesAsync();
            
            foreach (dynamic domainEvent in apprenticeship.FlushEvents())
            {
                await _domainEventDispatcher.Send(domainEvent);
            }
        }

        public async Task<ApprenticeshipDomainModel> Get(Guid key)
        {
            var apprenticeship = await DbContext.Apprenticeships
                .Include(x => x.Approvals)
                .Include(x => x.PriceHistories)
                .SingleAsync(x => x.Key == key);

            return _apprenticeshipFactory.GetExisting(apprenticeship);
        }

        public async Task Update(ApprenticeshipDomainModel apprenticeship)
        {
            _accountIdValidator.ValidateAccountId(apprenticeship);
            DbContext.Update(apprenticeship.GetEntity());

            await DbContext.SaveChangesAsync();
            
            foreach (dynamic domainEvent in apprenticeship.FlushEvents())
            {
                await _domainEventDispatcher.Send(domainEvent);
            }
        }
    }
}
