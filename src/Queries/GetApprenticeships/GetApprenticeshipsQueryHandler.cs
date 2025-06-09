using SFA.DAS.Learning.Domain.Repositories;

namespace SFA.DAS.Learning.Queries.GetApprenticeships
{
    public class GetApprenticeshipsQueryHandler : IQueryHandler<GetApprenticeshipsRequest, GetApprenticeshipsResponse>
    {
        private readonly IApprenticeshipQueryRepository _apprenticeshipQueryRepository;

        public GetApprenticeshipsQueryHandler(IApprenticeshipQueryRepository apprenticeshipQueryRepository)
        {
            _apprenticeshipQueryRepository = apprenticeshipQueryRepository;
        }

        public async Task<GetApprenticeshipsResponse> Handle(GetApprenticeshipsRequest query, CancellationToken cancellationToken = default)
        {
            var apprenticeships = await _apprenticeshipQueryRepository.GetAll(query.Ukprn, query.FundingPlatform);

            var response = new GetApprenticeshipsResponse(apprenticeships);

            return await Task.FromResult(response);
        }
    }
}