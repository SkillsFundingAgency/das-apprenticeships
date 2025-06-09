using Microsoft.EntityFrameworkCore;
using SFA.DAS.Learning.DataAccess;
using SFA.DAS.Learning.Domain.Apprenticeship;
using SFA.DAS.Learning.Domain.Factories;

namespace SFA.DAS.Learning.Domain.Repositories;

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
            .Include(x => x.PriceHistories)
            .Include(x => x.StartDateChanges)
            .Include(x => x.FreezeRequests)
            .Include(x => x.Episodes)
            .ThenInclude(y => y.Prices)
            .SingleAsync(x => x.Key == key);

        return _apprenticeshipFactory.GetExisting(apprenticeship);
    }

    public async Task<ApprenticeshipDomainModel?> Get(string uln, long approvalsApprenticeshipId)
    {
        var apprenticeship = await DbContext.Apprenticeships
            .Include(x => x.PriceHistories)
            .Include(x => x.StartDateChanges)
            .Include(x => x.FreezeRequests)
            .Include(x => x.Episodes)
            .ThenInclude(y => y.Prices)
            .SingleOrDefaultAsync(x => x.Uln == uln && x.ApprovalsApprenticeshipId == approvalsApprenticeshipId);
        return apprenticeship == null ? null : _apprenticeshipFactory.GetExisting(apprenticeship);
    }
    
    public async Task<ApprenticeshipDomainModel?> GetByUln(string uln)
    {
        var apprenticeship = await DbContext.Apprenticeships
            .Include(x => x.PriceHistories)
            .Include(x => x.StartDateChanges)
            .Include(x => x.FreezeRequests)
            .Include(x => x.Episodes)
            .ThenInclude(y => y.Prices)
            .SingleOrDefaultAsync(x => x.Uln == uln);

        if (apprenticeship == null)
        {
            return null;
        }

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