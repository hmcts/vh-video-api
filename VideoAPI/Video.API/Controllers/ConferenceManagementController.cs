using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Video.API.Mappings;
using VideoApi.Contract.Requests;
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
        /// <param name="request"></param>
        /// <returns>No Content status</returns>
        [HttpPost("{conferenceId}/start")]
        [SwaggerOperation(OperationId = "StartOrResumeVideoHearing")]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> StartVideoHearingAsync(Guid conferenceId, StartHearingRequest request)
        {
            try
            {
                var hearingLayout =
                    HearingLayoutMapper.MapLayoutToVideoHearingLayout(
                        request.Layout.GetValueOrDefault(HearingLayout.Dynamic));
                await _videoPlatformService.StartHearingAsync(conferenceId, hearingLayout);
                return Accepted();
            }
            catch (KinlyApiException ex)
            {
                _logger.LogError(ex, $"Unable to find start video hearing {conferenceId}");
                return StatusCode(ex.StatusCode, ex.Response);
            }
        }
        
        /// <summary>
        /// Pause a video hearing
        /// </summary>
        /// <param name="conferenceId">conference id</param>
        /// <returns>No Content status</returns>
        [HttpPost("{conferenceId}/pause")]
        [SwaggerOperation(OperationId = "PauseVideoHearing")]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> PauseVideoHearingAsync(Guid conferenceId)
        {
            try
            {
                await _videoPlatformService.PauseHearingAsync(conferenceId);
                return Accepted();
            }
            catch (KinlyApiException ex)
            {
                _logger.LogError(ex, $"Unable to pause video hearing {conferenceId}");
                return StatusCode(ex.StatusCode, ex.Response);
            }
        }
        
        /// <summary>
        /// End a video hearing
        /// </summary>
        /// <param name="conferenceId">conference id</param>
        /// <returns>No Content status</returns>
        [HttpPost("{conferenceId}/end")]
        [SwaggerOperation(OperationId = "EndVideoHearing")]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> EndVideoHearingAsync(Guid conferenceId)
        {
            try
            {
                await _videoPlatformService.EndHearingAsync(conferenceId);
                return Accepted();
            }
            catch (KinlyApiException ex)
            {
                _logger.LogError(ex, $"Unable to end video hearing {conferenceId}");
                return StatusCode(ex.StatusCode, ex.Response);
            }
        }
    }
}
