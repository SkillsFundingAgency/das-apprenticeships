using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Command;
using SFA.DAS.Apprenticeships.Command.ApproveStartDateChange;
using SFA.DAS.Apprenticeships.Command.CreateStartDateChange;
using SFA.DAS.Apprenticeships.Command.RejectStartDateChange;
using SFA.DAS.Apprenticeships.InnerApi.Requests;
using SFA.DAS.Apprenticeships.Queries;
using SFA.DAS.Apprenticeships.Queries.GetPendingStartDateChange;

namespace SFA.DAS.Apprenticeships.InnerApi.Controllers;

/// <summary>
/// Controller for handling requests related to start date changes
/// </summary>
[Route("")]
[ApiController]
[Authorize]
public class StartDateChangeController : ControllerBase
{
    private readonly IQueryDispatcher _queryDispatcher;
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly ILogger<StartDateChangeController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="StartDateChangeController"/> class.
    /// </summary>
    /// <param name="queryDispatcher"></param>
    /// <param name="commandDispatcher"></param>
    /// <param name="logger"></param>
    public StartDateChangeController(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, ILogger<StartDateChangeController> logger)
    {
        _queryDispatcher = queryDispatcher;
        _commandDispatcher = commandDispatcher;
        _logger = logger;
    }

    /// <summary>
    /// Create apprenticeship start date change
    /// </summary>
    /// <param name="apprenticeshipKey">The unique identifier of the apprenticeship</param>
    /// <param name="request">Details of the requested start date change.</param>
    /// <returns>Ok on success, Bad Request if neither employer or provider are set for initiator</returns>
    [HttpPost("{apprenticeshipKey}/startDateChange")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> CreateApprenticeshipStartDateChange(Guid apprenticeshipKey, [FromBody] PostCreateApprenticeshipStartDateChangeRequest request)
    {
        try
        {
            await _commandDispatcher.Send(new CreateStartDateChangeCommand(request.Initiator, apprenticeshipKey, request.UserId, request.ActualStartDate, request.PlannedEndDate, request.Reason));
            return Ok();
        }
        catch (ArgumentException exception)
        {
            _logger.LogError(exception, $"Bad Request, missing or invalid: {exception.ParamName}");
            return BadRequest();
        }
    }

    /// <summary>
    /// Gets the details of a pending start date change
    /// </summary>
    /// <param name="apprenticeshipKey">The unique identifier of the apprenticeship</param>
    /// <returns>Details of the pending start date change</returns>
    [HttpGet("{apprenticeshipKey}/startDateChange/pending")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetPendingStartDateChange(Guid apprenticeshipKey)
    {
        var request = new GetPendingStartDateChangeRequest(apprenticeshipKey);
        var response = await _queryDispatcher.Send<GetPendingStartDateChangeRequest, GetPendingStartDateChangeResponse>(request);

        if (!response.HasPendingStartDateChange)
        {
            return NotFound(response.PendingStartDateChange);
        }

        return Ok(response);
    }

    /// <summary>
    /// Approves a pending start date change
    /// </summary>
    /// <param name="apprenticeshipKey">The unique identifier of the apprenticeship</param>
    /// <param name="request">Details of the request for start date change approval</param>
    [HttpPatch("{apprenticeshipKey}/startDateChange/pending")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> ApproveStartDateChange(Guid apprenticeshipKey, [FromBody] ApproveStartDateChangeRequest request)
    {
        await _commandDispatcher.Send(new ApproveStartDateChangeCommand(apprenticeshipKey, request.UserId));
        return Ok();
    }

    /// <summary>
    /// Rejects a pending start date change
    /// </summary>
    /// <param name="apprenticeshipKey">The unique identifier of the apprenticeship</param>
    /// <param name="request">Details of the request for start date change rejection</param>
    [HttpPatch("{apprenticeshipKey}/startDateChange/reject")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> RejectStartDateChange(Guid apprenticeshipKey, [FromBody] RejectStartDateChangeRequest request)
    {
        await _commandDispatcher.Send(new RejectStartDateChangeCommand(apprenticeshipKey, request.Reason));
        return Ok();
    }
}