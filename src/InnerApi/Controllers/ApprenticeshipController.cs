using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.DataTransferObjects;
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

        /// <summary>
        /// Get apprenticeships
        /// </summary>
        /// <remarks>Gets all apprenticeships. The response from this endpoint only contains summary apprenticeship information.</remarks>
        /// <response code="200">Apprenticeships retrieved</response>
        [HttpGet("")]
        [ProducesResponseType(typeof(IEnumerable<Apprenticeship>), 200)]
        public async Task<IActionResult> GetAll(long ukprn, FundingPlatform? fundingPlatform)
        {
            var request = new GetApprenticeshipsRequest(ukprn, fundingPlatform);
            var response = await _queryDispatcher.Send<GetApprenticeshipsRequest, GetApprenticeshipsResponse>(request);

            return Ok(response.Apprenticeships);

            //TODO: write acceptance tests
        }
    }
}