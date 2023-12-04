﻿using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Command;
using SFA.DAS.Apprenticeships.Command.AddPriceHistory;
using SFA.DAS.Apprenticeships.DataTransferObjects;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.InnerApi.Requests;
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
        /// Create Apprenticeship Price Change Record
        /// </summary>
        /// <param name="apprenticeshipKey">The apprenticeship key</param>
        /// <param name="request">The request object with providerId, employerId, userId, trainingPrice, assessmentPrice, totalPrice, reason, effectiveFromDate</param>
        /// <response code="200">Apprenticeship Price Change Created</response>
        [HttpPost("{apprenticeshipKey}/priceHistory")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> CreateApprenticeshipPriceChange(Guid apprenticeshipKey, [FromBody] PostCreateApprenticeshipPriceChangeRequest request)
        {
            await _commandDispatcher.Send(new CreateApprenticeshipPriceChangeRequest(request.ProviderId, request.EmployerId, apprenticeshipKey, request.UserId, request.TrainingPrice, request.AssessmentPrice, request.TotalPrice, request.Reason, request.EffectiveFromDate));
            return Ok();
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
            var response = await _queryDispatcher.Send<GetApprenticeshipPriceRequest, GetApprenticeshipPriceResponse>(request);
            if (response.TotalPrice == null) return NotFound();
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
    }
}