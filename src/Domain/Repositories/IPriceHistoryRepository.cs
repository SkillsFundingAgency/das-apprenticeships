namespace SFA.DAS.Apprenticeships.Domain.Repositories;

public interface IPriceHistoryRepository
{
    Task Add(Apprenticeship.PriceHistory priceHistory);
}