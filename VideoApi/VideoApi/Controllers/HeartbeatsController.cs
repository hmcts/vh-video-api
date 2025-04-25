using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using VideoApi.DAL.Commands;
using VideoApi.Common.Logging;  


namespace VideoApi.Controllers;

[Consumes("application/json")]
[Route("conferences")]
[ApiController]
public class HeartbeatsController(ILogger<HeartbeatsController> logger, IBackgroundWorkerQueue backgroundWorkerQueue)
    : ControllerBase
{
    [HttpDelete("expiredHeartbeats")]
    [OpenApiOperation("RemoveHeartbeatsForConferences")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> RemoveHeartbeatsForConferencesAsync()
    {
        logger.LogRemovingOldHeartbeats();
        
        var removeHeartbeatsCommand = new RemoveHeartbeatsForConferencesCommand();
        await backgroundWorkerQueue.QueueBackgroundWorkItem(removeHeartbeatsCommand);
        
        logger.LogSuccessfullyRemovedHeartbeats();
        return NoContent();
    }
}
