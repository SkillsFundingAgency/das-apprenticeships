using SFA.DAS.Learning.Domain.Repositories;

namespace SFA.DAS.Learning.Queries.GetLearnings
{
    public class GetLearningsQueryHandler : IQueryHandler<GetLearningsRequest, GetLearningsResponse>
    {
        private readonly IApprenticeshipQueryRepository _apprenticeshipQueryRepository;

        public GetLearningsQueryHandler(IApprenticeshipQueryRepository apprenticeshipQueryRepository)
        {
            _apprenticeshipQueryRepository = apprenticeshipQueryRepository;
        }

        public async Task<GetLearningsResponse> Handle(GetLearningsRequest query, CancellationToken cancellationToken = default)
        {
            var apprenticeships = await _apprenticeshipQueryRepository.GetAll(query.Ukprn, query.FundingPlatform);

            var response = new GetLearningsResponse(apprenticeships);

            return await Task.FromResult(response);
        }
    }
}