﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Command;
using SFA.DAS.Apprenticeships.Command.CreateStartDateChange;
using SFA.DAS.Apprenticeships.InnerApi.Requests;
using SFA.DAS.Apprenticeships.Queries;

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
    /// <returns>Ok on success, Bad Request if neither employer or provider are set for intiator</returns>
    [HttpPost("{apprenticeshipKey}/startDateChange")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> CreateApprenticeshipStartDateChange(Guid apprenticeshipKey, [FromBody] PostCreateApprenticeshipStartDateChangeRequest request)
    {
        try
        {
            await _commandDispatcher.Send<CreateStartDateChangeCommand>(new CreateStartDateChangeCommand(request.Initiator, apprenticeshipKey, request.UserId, request.ActualStartDate, request.Reason));
            return Ok();
        }
        catch (ArgumentException exception)
        {
            _logger.LogError(exception, $"Bad Request, missing or invalid: {exception.ParamName}");
            return BadRequest();
        }

    }
}