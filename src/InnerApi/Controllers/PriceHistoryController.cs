using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Command;
using SFA.DAS.Apprenticeships.Command.ApprovePriceChange;
using SFA.DAS.Apprenticeships.Command.CreatePriceChange;
using SFA.DAS.Apprenticeships.Command.CancelPendingPriceChange;
using SFA.DAS.Apprenticeships.Command.RejectPendingPriceChange;
using SFA.DAS.Apprenticeships.InnerApi.Requests;
using SFA.DAS.Apprenticeships.Queries;
using SFA.DAS.Apprenticeships.Queries.GetPendingPriceChange;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.InnerApi.Responses;
using SFA.DAS.Apprenticeships.InnerApi.Identity.Authorization;

namespace SFA.DAS.Apprenticeships.InnerApi.Controllers
{
    /// <summary>
    /// Controller for handling requests related to price history
    /// </summary>
    [Route("")]
    [ApiController]
    [AuthorizeUserType(UserType.Provider | UserType.Employer)]
    public class PriceHistoryController : ControllerBase
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ILogger<PriceHistoryController> _logger;

        /// <summary>Initializes a new instance of the <see cref="PriceHistoryController"/> class.</summary>
        /// <param name="queryDispatcher"></param>
        /// <param name="commandDispatcher"></param>
        /// <param name="logger"></param>
        public PriceHistoryController(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, ILogger<PriceHistoryController> logger)
        {
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;
            _logger = logger;
        }

        /// <summary>
        /// Create apprenticeship price change
        /// </summary>
        /// <param name="apprenticeshipKey">The unique identifier of the apprenticeship</param>
        /// <param name="request">Details of the requested price change.</param>
        /// <returns>Ok on success, Bad Request if neither employer or provider are set for initiator</returns>
        [HttpPost("{apprenticeshipKey}/priceHistory")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> CreateApprenticeshipPriceChange(Guid apprenticeshipKey, [FromBody] PostCreateApprenticeshipPriceChangeRequest request)
        {
            try
            {
                var priceChangeStatus = await _commandDispatcher.Send<CreatePriceChangeCommand, ChangeRequestStatus>(new CreatePriceChangeCommand(request.Initiator, apprenticeshipKey, request.UserId, request.TrainingPrice, request.AssessmentPrice, request.TotalPrice, request.Reason, request.EffectiveFromDate));
                return Ok(new CreatePriceChangeResponse { PriceChangeStatus = priceChangeStatus.ToString()});
            }
            catch (ArgumentException exception)
            {
                _logger.LogError(exception, $"Bad Request, missing or invalid: {exception.ParamName}");
                return BadRequest();
            }
        }

        /// <summary>
        /// Gets the details of a pending price change
        /// </summary>
        /// <param name="apprenticeshipKey">The unique identifier of the apprenticeship</param>
        /// <returns>Details of the pending price change</returns>
        [HttpGet("{apprenticeshipKey}/priceHistory/pending")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetPendingPriceChange(Guid apprenticeshipKey)
        {
            var request = new GetPendingPriceChangeRequest(apprenticeshipKey);
            var response = await _queryDispatcher.Send<GetPendingPriceChangeRequest, GetPendingPriceChangeResponse>(request);

            if(!response.HasPendingPriceChange)
            {
                return NotFound(response.PendingPriceChange);
            }

            return Ok(response);
        }

        /// <summary>
        /// Approves a pending price change
        /// </summary>
        /// <param name="apprenticeshipKey">The unique identifier of the apprenticeship</param>
        /// <param name="request">Details of the approval</param>
        [HttpPatch("{apprenticeshipKey}/priceHistory/pending")]
		[ProducesResponseType(200)]
		public async Task<IActionResult> ApprovePriceChange(Guid apprenticeshipKey, [FromBody] ApprovePriceChangeRequest request)
		{
			var approver = await _commandDispatcher.Send<ApprovePriceChangeCommand, ApprovedBy>(new ApprovePriceChangeCommand(apprenticeshipKey, request.UserId, request.TrainingPrice, request.AssessmentPrice));
            return Ok(new { Approver = approver.ToString() });
        }

		/// <summary>
		/// Removes a pending price change
		/// </summary>
		/// <param name="apprenticeshipKey">The unique identifier of the apprenticeship</param>
		/// <returns>Ok result</returns>
		[HttpDelete("{apprenticeshipKey}/priceHistory/pending")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> CancelPendingPriceChange(Guid apprenticeshipKey)
        {
            var request = new CancelPendingPriceChangeRequest(apprenticeshipKey);
            await _commandDispatcher.Send(request);

            return Ok();
        }

        /// <summary>
        /// Rejects a pending price change
        /// </summary>
        /// <param name="apprenticeshipKey">The unique identifier of the apprenticeship</param>
        /// <param name="request">Details of the rejection</param>
        /// <returns>ok result</returns>
        [HttpPatch("{apprenticeshipKey}/priceHistory/pending/reject")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> RejectPendingPriceChange(Guid apprenticeshipKey, [FromBody]PatchRejectPriceChangeRequest request)
        {
            await _commandDispatcher.Send(new RejectPendingPriceChangeRequest(apprenticeshipKey, request.Reason));

            return Ok(new { Rejector = HttpContext.GetUserType()});
        }
    }
}
