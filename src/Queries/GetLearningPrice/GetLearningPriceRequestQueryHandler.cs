using SFA.DAS.Learning.Domain.Repositories;

namespace SFA.DAS.Learning.Queries.GetLearningPrice;

public class GetLearningPriceRequestQueryHandler : IQueryHandler<GetLearningPriceRequest, GetLearningPriceResponse?>
{
    private readonly ILearningQueryRepository _learningQueryRepository;

    public GetLearningPriceRequestQueryHandler(ILearningQueryRepository learningQueryRepository)
    {
        _learningQueryRepository = learningQueryRepository;
    }

    public async Task<GetLearningPriceResponse?> Handle(GetLearningPriceRequest query, CancellationToken cancellationToken = default)
    {
        var price = await _learningQueryRepository.GetPrice(query.ApprenticeshipKey);

        if (price == null) return null;

        return new GetLearningPriceResponse
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