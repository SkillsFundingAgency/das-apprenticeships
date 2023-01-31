using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.Queries;

namespace SFA.DAS.Apprenticeships.InnerApi.Controllers
{
    [ApiController]
    [Route("apprenticeships")]
    public class ApprenticeshipController : ControllerBase
    {
        private readonly IQueryDispatcher _queryDispatcher;

        public ApprenticeshipController(IQueryDispatcher queryDispatcher)
        {
            _queryDispatcher = queryDispatcher;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetAll(long ukprn, FundingPlatform fundingPlatform)
        {
            //TODO: Swagger docs
            //TODO: UNIT TEST
            var request = new GetApprenticeshipsRequest(ukprn, fundingPlatform);
            var response = await _queryDispatcher.Send<GetApprenticeshipsRequest, GetApprenticeshipsResponse>(request);

            return Ok(response.Apprenticeships);


            //TODO: write unit test for 
        }
    }
}