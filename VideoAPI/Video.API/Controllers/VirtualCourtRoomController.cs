using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Net;
using System.Threading.Tasks;
using VideoApi.Services;
using VideoApi.Services.Kinly;

namespace Video.API.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("virtualCourtRooms")]
    [ApiController]
    public class VirtualCourtRoomController : ControllerBase
    {
        private readonly IVideoPlatformService _videoPlatformService;
        private readonly ILogger<ConferenceController> _logger;

        public VirtualCourtRoomController(IVideoPlatformService videoPlatformService,
            ILogger<ConferenceController> logger)
        {
            _videoPlatformService = videoPlatformService;
            _logger = logger;
        }

        /// <summary>
        /// Remove a virtual court room 
        /// </summary>
        /// <param name="virtualCourtRoomId">The virtual court room id</param>
        /// <returns></returns>
        [HttpDelete("{virtualCourtRoomId}")]
        [SwaggerOperation(OperationId = "RemoveVirtualCourtRoom")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> RemoveVirtualCourtRoom(Guid virtualCourtRoomId)
        {
            _logger.LogDebug($"RemoveVirtualCourtRoom {virtualCourtRoomId}");
            
            try
            {
                await _videoPlatformService.DeleteVirtualCourtRoomAsync(virtualCourtRoomId.ToString());
            }
            catch (KinlyApiException e)
            {
                if (e.StatusCode == (int)HttpStatusCode.NotFound)
                {
                    _logger.LogError($"Unable to find virtual court room Id {virtualCourtRoomId}");
                    return NotFound();
                }
                else
                {
                    _logger.LogError($"Error to remove virtual court room Id {virtualCourtRoomId}, {e.Message}");
                    return BadRequest();
                }
            }
           
            _logger.LogInformation($"Successfully removed virtual court room {virtualCourtRoomId}");
            return NoContent();
        }

    }
}