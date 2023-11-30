//using SFA.DAS.Apprenticeships.DataAccess.Mappers.Apprenticeship;
//using SFA.DAS.Apprenticeships.Domain;
//using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
//using SFA.DAS.Apprenticeships.Domain.Factories;
//using SFA.DAS.Apprenticeships.Domain.Repositories;
//
//namespace SFA.DAS.Apprenticeships.DataAccess.Repositories;
//
//public class PriceHistoryRepository : IPriceHistoryRepository
//{
//    private readonly Lazy<ApprenticeshipsDataContext> _lazyContext;
//    private readonly IDomainEventDispatcher _domainEventDispatcher;
//    private readonly IApprenticeshipFactory _apprenticeshipFactory;
//    private ApprenticeshipsDataContext DbContext => _lazyContext.Value;
//
//    public PriceHistoryRepository(Lazy<ApprenticeshipsDataContext> dbContext, IDomainEventDispatcher domainEventDispatcher, IApprenticeshipFactory apprenticeshipFactory)
//    {
//        _lazyContext = dbContext;
//        _domainEventDispatcher = domainEventDispatcher;
//        _apprenticeshipFactory = apprenticeshipFactory;
//    }
//
//    public async Task Add(PriceHistory priceHistory)
//    {
//        var priceHistoryDataModel = priceHistory.GetModel().Map();
//        await DbContext.AddAsync(priceHistoryDataModel);
//        await DbContext.SaveChangesAsync();
//    }
//}