using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Mappings;
using VideoApi.Common.Logging;

namespace VideoApi.Controllers;

[Consumes("application/json")]
[Produces("application/json")]
[Route("end-of-day")]
[ApiController]
public class EndOfDayController(
    IQueryHandler queryHandler,
    ILogger<EndOfDayController> logger)
    : ControllerBase
{
    /// <summary>
    /// Get all active conferences.
    /// This includes conferences that are in progress or paused.
    /// This includes conferences that are closed but the participants are still in consultation.
    /// </summary>
    /// <returns></returns>
    [HttpGet("active-sessions")]
    [OpenApiOperation("GetActiveConferences")]
    [ProducesResponseType(typeof(List<ConferenceCoreResponse>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetActiveConferences()
    {
        logger.LogGettingActiveConferences();
        var query = new GetActiveConferencesQuery();
        var conferences = await queryHandler.Handle<GetActiveConferencesQuery, List<Conference>>(query);
        return Ok(conferences.Select(ConferenceCoreResponseMapper.Map));
    }
}
