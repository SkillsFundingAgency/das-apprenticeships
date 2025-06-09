using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Learning.Command;
using SFA.DAS.Learning.Enums;
using SFA.DAS.Learning.InnerApi.Identity.Authorization;
using SFA.DAS.Learning.InnerApi.Services;
using SFA.DAS.Learning.Queries;
using SFA.DAS.Learning.Queries.GetApprenticeshipKey;
using SFA.DAS.Learning.Queries.GetApprenticeshipKeyByApprenticeshipId;
using SFA.DAS.Learning.Queries.GetApprenticeshipPrice;
using SFA.DAS.Learning.Queries.GetApprenticeships;
using SFA.DAS.Learning.Queries.GetApprenticeshipsByAcademicYear;
using SFA.DAS.Learning.Queries.GetApprenticeshipStartDate;
using SFA.DAS.Learning.Queries.GetApprenticeshipsWithEpisodes;
using SFA.DAS.Learning.Queries.GetCurrentPartyIds;
using SFA.DAS.Learning.Queries.GetLearnerStatus;
using Apprenticeship = SFA.DAS.Learning.DataTransferObjects.Apprenticeship;

namespace SFA.DAS.Learning.InnerApi.Controllers;

/// <summary>
/// Controller for handling requests related to Apprenticeships
/// </summary>
[Route("")]
[ApiController]
[ControllerAuthorizeUserType(UserType.Provider | UserType.Employer)]
public class ApprenticeshipController : ControllerBase
{
    private readonly IQueryDispatcher _queryDispatcher;
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly ILogger<ApprenticeshipController> _logger;
    private readonly IPagedLinkHeaderService _pagedLinkHeaderService;

    /// <summary>Initializes a new instance of the <see cref="ApprenticeshipController"/> class.</summary>
    /// <param name="queryDispatcher">Gets data</param>
    /// <param name="commandDispatcher">updates data</param>
    /// <param name="logger">ILogger</param>
    /// <param name="pagedLinkHeaderService">IPagedQueryResultHelper</param>
    public ApprenticeshipController(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, ILogger<ApprenticeshipController> logger, IPagedLinkHeaderService pagedLinkHeaderService)
    {
        _queryDispatcher = queryDispatcher;
        _commandDispatcher = commandDispatcher;
        _logger = logger;
        _pagedLinkHeaderService = pagedLinkHeaderService;
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
    /// Get paginated apprenticeships for a provider between specified dates.
    /// </summary>
    /// <param name="ukprn">UkPrn filter value</param>
    /// <param name="academicYear">Academic year in yyyy format (e.g. 2425)</param>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>GetApprenticeshipsByAcademicYearResponse</returns>
    [HttpGet("{ukprn:long}/academicyears/{academicYear:int}/apprenticeships")]
    [ProducesResponseType(typeof(GetApprenticeshipsByAcademicYearResponse), 200)]
    [ActionAuthorizeUserType(UserType.ServiceAccount)]
    public async Task<IActionResult> GetByAcademicYear(long ukprn, int academicYear, [FromQuery] int page = 1, [FromQuery] int? pageSize = 20)
    {
        pageSize = pageSize.HasValue ? Math.Clamp(pageSize.Value, 1, 100) : pageSize;
        
        var request = new GetApprenticeshipsByAcademicYearRequest(ukprn, academicYear, page, pageSize);
        var response = await _queryDispatcher.Send<GetApprenticeshipsByAcademicYearRequest, GetApprenticeshipsByAcademicYearResponse>(request);

        var pageLinks = _pagedLinkHeaderService.GetPageLinks(request, response);
        
        Response?.Headers.Add(pageLinks);

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
    /// <param name="collectionYear">Collection Year</param>
    /// <param name="collectionPeriod">Collection Period</param>
    /// <returns>GetApprenticeshipResponse containing apprenticeship, episode, & price data</returns>
    [HttpGet("{ukprn}/{collectionYear}/{collectionPeriod}")]
    [ProducesResponseType(200)]
    [ActionAuthorizeUserType(UserType.ServiceAccount)]
    public async Task<IActionResult> GetApprenticeshipsForFm36(long ukprn, short collectionYear, byte collectionPeriod)
    {
        var request = new GetApprenticeshipsWithEpisodesRequest { Ukprn = ukprn, CollectionYear = collectionYear, CollectionPeriod = collectionPeriod };
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
    [ActionAuthorizeUserType(UserType.ServiceAccount | UserType.Provider | UserType.Employer)]
    public async Task<IActionResult> GetCurrentPartyIds(Guid apprenticeshipKey)
    {
        var request = new GetCurrentPartyIdsRequest { ApprenticeshipKey = apprenticeshipKey };
        var response = await _queryDispatcher.Send<GetCurrentPartyIdsRequest, GetCurrentPartyIdsResponse?>(request);
        if (response == null) return NotFound();
        return Ok(response);
    }
}