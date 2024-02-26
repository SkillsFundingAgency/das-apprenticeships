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

namespace SFA.DAS.Apprenticeships.InnerApi.Controllers
{
    /// <summary>
    /// Controller for handling requests related to price history
    /// </summary>
    [Route("")]
    [ApiController]
	[Authorize]
	public class PriceHistoryController : ControllerBase
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;

        /// <summary>Initializes a new instance of the <see cref="PriceHistoryController"/> class.</summary>
        /// <param name="queryDispatcher"></param>
        /// <param name="commandDispatcher"></param>
        public PriceHistoryController(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher)
        {
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;
        }

        /// <summary>
        /// Create apprenticeship price change
        /// </summary>
        /// <param name="apprenticeshipKey">The unique identifier of the apprenticeship</param>
        /// <param name="request">Details of the requested price change</param>
        /// <returns>Ok result</returns>
        [HttpPost("{apprenticeshipKey}/priceHistory")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> CreateApprenticeshipPriceChange(Guid apprenticeshipKey, [FromBody] PostCreateApprenticeshipPriceChangeRequest request)
        {
            await _commandDispatcher.Send(new CreatePriceChangeCommand(apprenticeshipKey, request.UserId, request.TrainingPrice, request.AssessmentPrice, request.TotalPrice, request.Reason, request.EffectiveFromDate));
            return Ok();
        }

        [HttpPatch("{apprenticeshipKey}/priceHistory/pending")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> ApprovePriceChange(Guid apprenticeshipKey, [FromBody] ApprovePriceChangeRequest request)
        {
            await _commandDispatcher.Send(new ApprovePriceChangeCommand(apprenticeshipKey, request.UserId));
            return Ok();
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

            return Ok();
        }
    }
}
