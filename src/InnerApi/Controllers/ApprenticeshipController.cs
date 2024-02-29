using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.DataTransferObjects;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.Queries;
using SFA.DAS.Apprenticeships.Queries.GetApprenticeshipKey;
using SFA.DAS.Apprenticeships.Queries.GetApprenticeshipKeyByApprenticeshipId;
using SFA.DAS.Apprenticeships.Queries.GetApprenticeshipPrice;
using SFA.DAS.Apprenticeships.Queries.GetApprenticeships;

namespace SFA.DAS.Apprenticeships.InnerApi.Controllers
{
    /// <summary>
    /// Controller for handling requests related to Apprenticeships
    /// </summary>
    [Route("")]
    [ApiController]
    [Authorize]
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
        [HttpGet("{ukprn}/apprenticeships")]
        [ProducesResponseType(typeof(IEnumerable<Apprenticeship>), 200)]
        public async Task<IActionResult> GetAll(long ukprn, FundingPlatform? fundingPlatform)
        {
            var request = new GetApprenticeshipsRequest(ukprn, fundingPlatform);
            var response = await _queryDispatcher.Send<GetApprenticeshipsRequest, GetApprenticeshipsResponse>(request);

            return Ok(response.Apprenticeships);
        }

        /// <summary>
        /// Get Apprenticeship Price
        /// </summary>
        /// <param name="apprenticeshipKey"></param>
        /// <returns>Apprenticeship Price</returns>
        [HttpGet("{apprenticeshipKey}/price")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetApprenticeshipPrice(Guid apprenticeshipKey)
        {
            var request = new GetApprenticeshipPriceRequest{ ApprenticeshipKey = apprenticeshipKey };
            var response = await _queryDispatcher.Send<GetApprenticeshipPriceRequest, GetApprenticeshipPriceResponse?>(request);
            if (response == null) return NotFound();
            return Ok(response);
        }

        /// <summary>
        /// Get Apprenticeship Key
        /// </summary>
        /// <param name="apprenticeshipHashedId">This should be the hashed id for the apprenticeship not the commitment</param>
        /// <returns>Apprenticeship Key</returns>
        [HttpGet("{apprenticeshipHashedId}/key")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetApprenticeshipKey(string apprenticeshipHashedId)
        {
            var request = new GetApprenticeshipKeyRequest { ApprenticeshipHashedId = apprenticeshipHashedId };
            var response = await _queryDispatcher.Send<GetApprenticeshipKeyRequest, GetApprenticeshipKeyResponse>(request);
            if (response.ApprenticeshipKey == null) return NotFound();
            return Ok(response.ApprenticeshipKey);
        }

        /// <summary>
        /// Get Apprenticeship Key
        /// </summary>
        /// <param name="apprenticeshipId">This should be the id for the apprenticeship not the commitment</param>
        /// <returns>Apprenticeship Key</returns>
        [HttpGet("{apprenticeshipId}/key2")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetApprenticeshipKeyByApprenticeshipId(long apprenticeshipId)
        {
	        var request = new GetApprenticeshipKeyByApprenticeshipIdRequest { ApprenticeshipId = apprenticeshipId };
	        var response = await _queryDispatcher.Send<GetApprenticeshipKeyByApprenticeshipIdRequest, GetApprenticeshipKeyByApprenticeshipIdResponse>(request);
	        if (response.ApprenticeshipKey == null) return NotFound();
	        return Ok(response.ApprenticeshipKey);
        }
	}
}