using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using VideoApi.Common.Security.Supplier.Base;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Mappings;
using VideoApi.Services;

namespace VideoApi.Controllers;

[Consumes("application/json")]
[Produces("application/json")]
[Route("end-of-day")]
[ApiController]
public class EndOfDayController : ControllerBase
{
    private readonly IQueryHandler _queryHandler;
    private readonly SupplierConfiguration _supplierConfiguration;
    private readonly ILogger<EndOfDayController> _logger;
    
    public EndOfDayController(IQueryHandler queryHandler, ILogger<EndOfDayController> logger,
        ISupplierApiSelector supplierLocator)
    {
        _queryHandler = queryHandler;
        _logger = logger;
        _supplierConfiguration = supplierLocator.GetSupplierConfiguration();
    }
    
    /// <summary>
    /// Get all active conferences.
    /// This includes conferences that are in progress or paused.
    /// This includes conferences that are closed but the participants are still in consultation.
    /// </summary>
    /// <returns></returns>
    [HttpGet("active-sessions")]
    [OpenApiOperation("GetActiveConferences")]
    [ProducesResponseType(typeof(List<ConferenceForAdminResponse>), (int) HttpStatusCode.OK)]
    public async Task<IActionResult> GetActiveConferences()
    {
        _logger.LogDebug("Getting all active conferences");
        var query = new GetActiveConferencesQuery();
        var conferences = await _queryHandler.Handle<GetActiveConferencesQuery, List<Conference>>(query);
        
        var response = conferences
            .Select(conference =>  ConferenceForAdminResponseMapper.MapConferenceToSummaryResponse(conference, _supplierConfiguration))
            .ToList();
        
        return Ok(response);
    }
    
}
