using SFA.DAS.Apprenticeships.Domain.Repositories;

namespace SFA.DAS.Apprenticeships.Queries.GetApprenticeshipPrice;

public class GetApprenticeshipPriceRequestQueryHandler : IQueryHandler<GetApprenticeshipPriceRequest, GetApprenticeshipPriceResponse>
{
    private readonly IApprenticeshipQueryRepository _apprenticeshipQueryRepository;

    public GetApprenticeshipPriceRequestQueryHandler(IApprenticeshipQueryRepository apprenticeshipQueryRepository)
    {
        _apprenticeshipQueryRepository = apprenticeshipQueryRepository;
    }

    public async Task<GetApprenticeshipPriceResponse> Handle(GetApprenticeshipPriceRequest query, CancellationToken cancellationToken = default)
    {
        var priceHistory = await _apprenticeshipQueryRepository.GetPriceHistory(query.ApprenticeshipKey);
        var latestPrice = priceHistory.MaxBy(x => x.EffectiveFromDate);

        return new GetApprenticeshipPriceResponse
        {
            ApprenticeshipKey = query.ApprenticeshipKey,
            TrainingPrice = latestPrice?.TrainingPrice,
            AssessmentPrice = latestPrice?.AssessmentPrice,
            TotalPrice = latestPrice.TotalPrice
        };
    }
}