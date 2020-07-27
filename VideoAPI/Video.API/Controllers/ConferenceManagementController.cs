using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using VideoApi.Services.Contracts;
using VideoApi.Services.Kinly;

namespace Video.API.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [SwaggerTag("Conference Management")]
    [Route("conferences")]
    [ApiController]
    public class ConferenceManagementController : ControllerBase
    {
        private readonly IVideoPlatformService _videoPlatformService;
        private readonly ILogger<ConferenceManagementController> _logger;

        public ConferenceManagementController(IVideoPlatformService videoPlatformService,
            ILogger<ConferenceManagementController> logger)
        {
            _videoPlatformService = videoPlatformService;
            _logger = logger;
        }

        /// <summary>
        /// Start or resume a video hearing
        /// </summary>
        /// <param name="conferenceId">conference id</param>
        /// <returns>No Content status</returns>
        [HttpPost("{conferenceId}/start")]
        [SwaggerOperation(OperationId = "StartOrResumeVideoHearing")]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> StartVideoHearingAsync(Guid conferenceId)
        {
            try
            {
                await _videoPlatformService.StartHearingAsync(conferenceId);
                return Accepted();
            }
            catch (KinlyApiException ex)
            {
                _logger.LogError(ex, $"Unable to find start video hearing {conferenceId}");
                return StatusCode(ex.StatusCode, ex.Response);
            }
        }
    }
}
