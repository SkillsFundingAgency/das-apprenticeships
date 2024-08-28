using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Command;
using SFA.DAS.Apprenticeships.DataTransferObjects;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.Queries;
using SFA.DAS.Apprenticeships.Queries.GetApprenticeship;
using SFA.DAS.Apprenticeships.Queries.GetApprenticeshipKey;
using SFA.DAS.Apprenticeships.Queries.GetApprenticeshipKeyByApprenticeshipId;
using SFA.DAS.Apprenticeships.Queries.GetApprenticeshipPaymentStatus;
using SFA.DAS.Apprenticeships.Queries.GetApprenticeshipPrice;
using SFA.DAS.Apprenticeships.Queries.GetApprenticeships;
using SFA.DAS.Apprenticeships.Queries.GetApprenticeshipStartDate;
using SFA.DAS.Apprenticeships.Queries.GetLearnerStatus;
using Apprenticeship = SFA.DAS.Apprenticeships.DataTransferObjects.Apprenticeship;

namespace SFA.DAS.Apprenticeships.InnerApi.Controllers;

/// <summary>
/// Controller for handling requests related to Apprenticeships
/// </summary>
[Route("")]
[ApiController]
[Authorize]
public class ApprenticeshipController : ControllerBase
{
    private readonly IQueryDispatcher _queryDispatcher;
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly ILogger<ApprenticeshipController> _logger;

    /// <summary>Initializes a new instance of the <see cref="ApprenticeshipController"/> class.</summary>
    /// <param name="queryDispatcher">Gets data</param>
    /// <param name="commandDispatcher">updates data</param>
    /// <param name="logger">ILogger</param>
    public ApprenticeshipController(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, ILogger<ApprenticeshipController> logger)
    {
        _queryDispatcher = queryDispatcher;
        _commandDispatcher = commandDispatcher;
        _logger = logger;
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
    /// Get Apprenticeship Start Date
    /// </summary>
    /// <param name="apprenticeshipKey">Guid</param>
    /// <returns>Apprenticeship Start Date or NotFound</returns>
    [HttpGet("{apprenticeshipKey}/startDate")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetStartDate(Guid apprenticeshipKey)
    {
        var request = new GetApprenticeshipStartDateRequest { ApprenticeshipKey = apprenticeshipKey };
        var response = await _queryDispatcher.Send<GetApprenticeshipStartDateRequest, GetApprenticeshipStartDateResponse?>(request);
        if (response == null || response.ApprenticeshipStartDate == null) return NotFound();
        return Ok(response.ApprenticeshipStartDate);
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
        if (response.ApprenticeshipKey == null)
        {
            _logger.LogInformation("{p1} could not be found.", nameof(response.ApprenticeshipKey));
            return NotFound();
        }
        return Ok(response.ApprenticeshipKey);
    }

    /// <summary>
    /// Gets the Learner Status of the apprenticeship
    /// </summary>
    /// <param name="apprenticeshipKey">Guid</param>
    /// <returns>GetLearnerStatusResponse containing LearnerStatus</returns>
    [HttpGet("{apprenticeshipKey}/LearnerStatus")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetLearnerStatus(Guid apprenticeshipKey)
    {
        var request = new GetLearnerStatusRequest { ApprenticeshipKey = apprenticeshipKey };
        var response = await _queryDispatcher.Send<GetLearnerStatusRequest, GetLearnerStatusResponse?>(request);
        if (response == null) return NotFound();
        return Ok(response);
    }

    /// <summary>
    /// Gets the apprenticeship with episode & price data
    /// </summary>
    /// <param name="ukprn">Ukprn</param>
    /// <param name="uln">Uln</param>
    /// <returns>GetApprenticeshipResponse containing apprenticeship, episode, & price data</returns>
    [HttpGet("{ukprn}/{uln}")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> Get(long ukprn, string uln)
    {
        var request = new GetApprenticeshipRequest { Ukprn = ukprn, Uln = uln };
        var response = await _queryDispatcher.Send<GetApprenticeshipRequest, GetApprenticeshipResponse?>(request);
        if (response == null) return NotFound();
        return Ok(response);
    }
}