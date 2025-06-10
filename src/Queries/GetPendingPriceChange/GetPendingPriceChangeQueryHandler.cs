using SFA.DAS.Learning.Domain.Repositories;

namespace SFA.DAS.Learning.Queries.GetPendingPriceChange;

public class GetPendingPriceChangeQueryHandler : IQueryHandler<GetPendingPriceChangeRequest, GetPendingPriceChangeResponse?>
{
    private readonly ILearningQueryRepository _learningQueryRepository;

    public GetPendingPriceChangeQueryHandler(ILearningQueryRepository learningQueryRepository)
    {
        _learningQueryRepository = learningQueryRepository;
    }

    public async Task<GetPendingPriceChangeResponse> Handle(GetPendingPriceChangeRequest query, CancellationToken cancellationToken = default)
    {
        var pendingPriceChange = await _learningQueryRepository.GetPendingPriceChange(query.ApprenticeshipKey);

        if (pendingPriceChange == null) return new GetPendingPriceChangeResponse { HasPendingPriceChange = false };

        return new GetPendingPriceChangeResponse
        {
            HasPendingPriceChange = true,
            PendingPriceChange = pendingPriceChange
        };
    }
}