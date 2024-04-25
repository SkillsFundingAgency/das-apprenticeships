using Microsoft.EntityFrameworkCore;
using SFA.DAS.Apprenticeships.DataAccess;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Factories;

namespace SFA.DAS.Apprenticeships.Domain.Repositories;

public class ApprenticeshipRepository : IApprenticeshipRepository
{
    private readonly Lazy<ApprenticeshipsDataContext> _lazyContext;
    private IDomainEventDispatcher _domainEventDispatcher;
    private readonly IApprenticeshipFactory _apprenticeshipFactory;
    private readonly IAccountIdAuthorizer _accountIdAuthorizer;
    private ApprenticeshipsDataContext DbContext => _lazyContext.Value;

    public ApprenticeshipRepository(Lazy<ApprenticeshipsDataContext> dbContext, IDomainEventDispatcher domainEventDispatcher, IApprenticeshipFactory apprenticeshipFactory, IAccountIdAuthorizer accountIdAuthorizer)
    {
        _lazyContext = dbContext;
        _domainEventDispatcher = domainEventDispatcher;
        _apprenticeshipFactory = apprenticeshipFactory;
        _accountIdAuthorizer = accountIdAuthorizer;
    }

    public async Task Add(ApprenticeshipDomainModel apprenticeship)
    {
        var entity = apprenticeship.GetEntity();
        _accountIdAuthorizer.AuthorizeAccountId(entity);
        await DbContext.AddAsync(entity);
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
                .Include(x => x.StartDateChanges)
            .SingleAsync(x => x.Key == key);

        return _apprenticeshipFactory.GetExisting(apprenticeship);
    }

    public async Task Update(ApprenticeshipDomainModel apprenticeship)
    {
        var entity = apprenticeship.GetEntity();
        _accountIdAuthorizer.AuthorizeAccountId(entity);
        DbContext.Update(entity);

        await DbContext.SaveChangesAsync();
            
        foreach (dynamic domainEvent in apprenticeship.FlushEvents())
        {
            await _domainEventDispatcher.Send(domainEvent);
        }
    }
}