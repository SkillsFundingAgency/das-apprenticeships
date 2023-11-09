using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Command;
using SFA.DAS.Apprenticeships.Command.AddPriceHistory;
using SFA.DAS.Apprenticeships.DataTransferObjects;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.Queries;
using SFA.DAS.Apprenticeships.Queries.GetApprenticeshipKey;
using SFA.DAS.Apprenticeships.Queries.GetApprenticeshipPrice;
using SFA.DAS.Apprenticeships.Queries.GetApprenticeships;

namespace SFA.DAS.Apprenticeships.InnerApi.Controllers
{
    /// <summary>
    /// Controller for handling requests related to Apprenticeships
    /// </summary>
    [Route("")]
    [ApiController]
    public class ApprenticeshipController : ControllerBase
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;

        /// <summary>Initializes a new instance of the <see cref="ApprenticeshipController"/> class.</summary>
        /// <param name="queryDispatcher"></param>
        public ApprenticeshipController(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher)
        {
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;
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
        /// 
        /// </summary>
        /// <param name="ukprn"></param>
        /// <param name="employerId"></param>
        /// <param name="apprenticeshipKey"></param>
        /// <param name="userId"></param>
        /// <param name="trainingPrice"></param>
        /// <param name="assessmentPrice"></param>
        /// <param name="totalPrice"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        [HttpPost("apprenticeshipPriceChange")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> CreateApprenticeshipPriceChange(long? ukprn, long? employerId, Guid apprenticeshipKey,
            string userId, decimal? trainingPrice, decimal? assessmentPrice, decimal totalPrice, string reason)
        {
            await _commandDispatcher.Send(new CreateApprenticeshipPriceChangeRequest(ukprn, employerId, apprenticeshipKey, userId, trainingPrice, assessmentPrice, totalPrice, reason));
            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="apprenticeshipKey"></param>
        /// <returns></returns>
        [HttpGet("{apprenticeshipKey}/price")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetApprenticeshipPrice(Guid apprenticeshipKey)
        {
            var request = new GetApprenticeshipPriceRequest{ ApprenticeshipKey = apprenticeshipKey };
            var response = await _queryDispatcher.Send<GetApprenticeshipPriceRequest, GetApprenticeshipPriceResponse>(request);
            return Ok(response);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="apprenticeshipHashedId"></param>
        /// <returns></returns>
        [HttpGet("{apprenticeshipHashedId}/key")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetApprenticeshipKey(string apprenticeshipHashedId)
        {
            var request = new GetApprenticeshipKeyRequest { ApprenticeshipHashedId = apprenticeshipHashedId };
            var response = await _queryDispatcher.Send<GetApprenticeshipKeyRequest, GetApprenticeshipKeyResponse>(request);
            if (response.ApprenticeshipKey == null)
                return NotFound();
            return Ok(response.ApprenticeshipKey);
        }
    }
}