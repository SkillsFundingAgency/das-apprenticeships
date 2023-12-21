using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Command;
using SFA.DAS.Apprenticeships.Command.AddPriceHistory;
using SFA.DAS.Apprenticeships.Command.CancelPendingPriceChange;
using SFA.DAS.Apprenticeships.InnerApi.Requests;
using SFA.DAS.Apprenticeships.Queries;
using SFA.DAS.Apprenticeships.Queries.GetPendingPriceChange;

namespace SFA.DAS.Apprenticeships.InnerApi.Controllers
{
    [Route("")]
    [ApiController]
    public class PriceHistoryController : ControllerBase
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;

        public PriceHistoryController(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher)
        {
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;
        }

        [HttpPost("{apprenticeshipKey}/priceHistory")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> CreateApprenticeshipPriceChange(Guid apprenticeshipKey, [FromBody] PostCreateApprenticeshipPriceChangeRequest request)
        {
            await _commandDispatcher.Send(new CreateApprenticeshipPriceChangeRequest(request.ProviderId, request.EmployerId, apprenticeshipKey, request.UserId, request.TrainingPrice, request.AssessmentPrice, request.TotalPrice, request.Reason, request.EffectiveFromDate));
            return Ok();
        }

        [HttpGet("{apprenticeshipKey}/priceHistory/pending")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetPendingPriceChange(Guid apprenticeshipKey)
        {
            var request = new GetPendingPriceChangeRequest(apprenticeshipKey);
            var response = await _queryDispatcher.Send<GetPendingPriceChangeRequest, GetPendingPriceChangeResponse>(request);

            return Ok(response);
        }

        [HttpDelete("{apprenticeshipKey}/priceHistory/pending")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> CancelPendingPriceChange(Guid apprenticeshipKey)
        {
            var request = new CancelPendingPriceChangeRequest(apprenticeshipKey);
            await _commandDispatcher.Send(request);

            return Ok();
        }
    }
}
