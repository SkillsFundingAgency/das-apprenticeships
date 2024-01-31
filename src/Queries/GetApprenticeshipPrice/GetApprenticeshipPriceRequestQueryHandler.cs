using SFA.DAS.Apprenticeships.Domain.Repositories;

namespace SFA.DAS.Apprenticeships.Queries.GetApprenticeshipPrice;

public class GetApprenticeshipPriceRequestQueryHandler : IQueryHandler<GetApprenticeshipPriceRequest, GetApprenticeshipPriceResponse?>
{
    private readonly IApprenticeshipQueryRepository _apprenticeshipQueryRepository;

    public GetApprenticeshipPriceRequestQueryHandler(IApprenticeshipQueryRepository apprenticeshipQueryRepository)
    {
        _apprenticeshipQueryRepository = apprenticeshipQueryRepository;
    }

    public async Task<GetApprenticeshipPriceResponse?> Handle(GetApprenticeshipPriceRequest query, CancellationToken cancellationToken = default)
    {
        var price = await _apprenticeshipQueryRepository.GetPrice(query.ApprenticeshipKey);

        if (price == null) return null;

        return new GetApprenticeshipPriceResponse
        {
            ApprenticeshipKey = query.ApprenticeshipKey,
            TrainingPrice = price.TrainingPrice,
            AssessmentPrice = price.AssessmentPrice,
            TotalPrice = price.TotalPrice,
            FundingBandMaximum = price.FundingBandMaximum,
            ApprenticeshipActualStartDate = price.ApprenticeshipActualStartDate,
            ApprenticeshipPlannedEndDate = price.ApprenticeshipPlannedEndDate,
            AccountLegalEntityId = price.AccountLegalEntityId,
            UKPRN = price.UKPRN
        };
    }
}