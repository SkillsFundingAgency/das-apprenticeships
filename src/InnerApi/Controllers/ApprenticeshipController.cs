using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.DataTransferObjects;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.Queries;
using SFA.DAS.Apprenticeships.Queries.GetApprenticeships;

namespace SFA.DAS.Apprenticeships.InnerApi.Controllers
{
    /// <summary>
    /// Controller for handling requests related to Apprenticeships
    /// </summary>
    [Route("{ukprn}")]
    [ApiController]
    public class ApprenticeshipController : ControllerBase
    {
        private readonly IQueryDispatcher _queryDispatcher;

        /// <summary>Initializes a new instance of the <see cref="ApprenticeshipController"/> class.</summary>
        /// <param name="queryDispatcher"></param>
        public ApprenticeshipController(IQueryDispatcher queryDispatcher)
        {
            _queryDispatcher = queryDispatcher;
        }

        /// <summary>
        /// Get apprenticeships
        /// </summary>
        /// <param name="ukprn">Filter by training provider using the unique provider number.</param>
        /// <param name="fundingPlatform" >Filter by the funding platform. This parameter is optional.</param>
        /// <remarks>Gets all apprenticeships. The response from this endpoint only contains summary apprenticeship information.</remarks>
        /// <response code="200">Apprenticeships retrieved</response>
        [HttpGet("apprenticeships")]
        [ProducesResponseType(typeof(IEnumerable<Apprenticeship>), 200)]
        public async Task<IActionResult> GetAll(long ukprn, FundingPlatform? fundingPlatform)
        {
            var request = new GetApprenticeshipsRequest(ukprn, fundingPlatform);
            var response = await _queryDispatcher.Send<GetApprenticeshipsRequest, GetApprenticeshipsResponse>(request);

            return Ok(response.Apprenticeships);
        }
    }
}