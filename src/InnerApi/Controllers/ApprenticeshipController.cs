using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Command;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.InnerApi.Identity.Authorization;
using SFA.DAS.Apprenticeships.InnerApi.Responses;
using SFA.DAS.Apprenticeships.Queries;
using SFA.DAS.Apprenticeships.Queries.GetApprenticeshipKey;
using SFA.DAS.Apprenticeships.Queries.GetApprenticeshipKeyByApprenticeshipId;
using SFA.DAS.Apprenticeships.Queries.GetApprenticeshipPrice;
using SFA.DAS.Apprenticeships.Queries.GetApprenticeshipsByAcademicYear;
using SFA.DAS.Apprenticeships.Queries.GetApprenticeshipStartDate;
using SFA.DAS.Apprenticeships.Queries.GetApprenticeshipsWithEpisodes;
using SFA.DAS.Apprenticeships.Queries.GetCurrentPartyIds;
using SFA.DAS.Apprenticeships.Queries.GetLearnerStatus;
using Apprenticeship = SFA.DAS.Apprenticeships.DataTransferObjects.Apprenticeship;

namespace SFA.DAS.Apprenticeships.InnerApi.Controllers;

/// <summary>
/// Controller for handling requests related to Apprenticeships
/// </summary>
[Route("")]
[ApiController]
[AuthorizeUserType(UserType.Provider | UserType.Employer)]
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
        var request = new Queries.GetApprenticeships.GetApprenticeshipsRequest(ukprn, fundingPlatform);
        var response = await _queryDispatcher.Send<Queries.GetApprenticeships.GetApprenticeshipsRequest, Queries.GetApprenticeships.GetApprenticeshipsResponse>(request);

        return Ok(response.Apprenticeships);
    }

    /// <summary>
    /// Get paginated apprenticeships for a provider for a specific academic year.
    /// </summary>
    /// <param name="ukprn">Filter by training provider using the unique provider number.</param>
    /// <param name="academicYear">Academic year in ISO date format (e.g. 2025-09-01).</param>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns code="200">GetApprenticeshipsByAcademicYearResponse</returns>
    [HttpGet("{ukprn:long}/academicyears/{academicYear}/apprenticeships")]
    [ProducesResponseType(typeof(GetApprenticeshipsByAcademicYearResponse), 200)]
    [AuthorizeUserType(UserType.Provider)]
    [AuthorizeUserType(UserType.ServiceAccount, UserTypeRequirement.AuthorizeMode.Override)]
    public async Task<IActionResult> GetForAcademicYear(long ukprn, string academicYear, [FromQuery] int page = 1, [FromQuery] int? pageSize = null)
    {
        var validDate = DateTime.TryParse(academicYear, out var academicYearValue);

        if (!validDate)
        {
            return new BadRequestResult();
        }
        
        var request = new GetApprenticeshipsByAcademicYearRequest(ukprn, academicYearValue, page, pageSize);
        var response = await _queryDispatcher.Send<GetApprenticeshipsByAcademicYearRequest, GetApprenticeshipsByAcademicYearResponse>(request);
        
        return Ok(response);
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
        var request = new GetApprenticeshipPriceRequest { ApprenticeshipKey = apprenticeshipKey };
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
    /// Gets all apprenticeships for a given provider with episode & price data
    /// </summary>
    /// <param name="ukprn">Ukprn</param>
    /// <returns>GetApprenticeshipResponse containing apprenticeship, episode, & price data</returns>
    [HttpGet("{ukprn}")]
    [ProducesResponseType(200)]
    [AuthorizeUserType(UserType.ServiceAccount, UserTypeRequirement.AuthorizeMode.Override)]
    public async Task<IActionResult> GetApprenticeships(long ukprn)
    {
        var request = new GetApprenticeshipsWithEpisodesRequest { Ukprn = ukprn };
        var response = await _queryDispatcher.Send<GetApprenticeshipsWithEpisodesRequest, GetApprenticeshipsWithEpisodesResponse?>(request);
        if (response == null) return NotFound();
        return Ok(response);
    }


    /// <summary>
    /// Get the provider and employer ids for the current state of the apprenticeship, this may differ from the original owner
    /// </summary>
    /// <param name="apprenticeshipKey">Guid</param>
    /// <returns>Provider and employer ids</returns>
    [HttpGet("{apprenticeshipKey}/currentPartyIds")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetCurrentPartyIds(Guid apprenticeshipKey)
    {
        var request = new GetCurrentPartyIdsRequest { ApprenticeshipKey = apprenticeshipKey };
        var response = await _queryDispatcher.Send<GetCurrentPartyIdsRequest, GetCurrentPartyIdsResponse?>(request);
        if (response == null) return NotFound();
        return Ok(response);
    }
}