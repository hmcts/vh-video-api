using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using VideoApi.Contract.Requests;
using VideoApi.Domain.Enums;
using VideoApi.Mappings;
using VideoApi.Services.Contracts;
using VideoApi.Services.Kinly;

namespace VideoApi.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [OpenApiTag("Conference Management")]
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
        [OpenApiOperation("StartOrResumeVideoHearing")]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> StartVideoHearingAsync(Guid conferenceId, StartHearingRequest request)
        {
            try
            {
                _logger.LogDebug("Attempting to start hearing");
                var hearingLayout =
                    HearingLayoutMapper.MapLayoutToVideoHearingLayout(
                        request.Layout.GetValueOrDefault(HearingLayout.Dynamic));
                await _videoPlatformService.StartHearingAsync(conferenceId, hearingLayout);
                return Accepted();
            }
            catch (KinlyApiException ex)
            {
                _logger.LogError(ex, "Error from Kinly API. Unable to start video hearing");
                return StatusCode(ex.StatusCode, ex.Response);
            }
        }
        
        /// <summary>
        /// Pause a video hearing
        /// </summary>
        /// <param name="conferenceId">conference id</param>
        /// <returns>No Content status</returns>
        [HttpPost("{conferenceId}/pause")]
        [OpenApiOperation("PauseVideoHearing")]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> PauseVideoHearingAsync(Guid conferenceId)
        {
            try
            {
                _logger.LogDebug("Attempting to pause hearing");
                await _videoPlatformService.PauseHearingAsync(conferenceId);
                return Accepted();
            }
            catch (KinlyApiException ex)
            {
                _logger.LogError(ex, "Error from Kinly API. Unable to pause video hearing");
                return StatusCode(ex.StatusCode, ex.Response);
            }
        }
        
        /// <summary>
        /// End a video hearing
        /// </summary>
        /// <param name="conferenceId">conference id</param>
        /// <returns>No Content status</returns>
        [HttpPost("{conferenceId}/end")]
        [OpenApiOperation("EndVideoHearing")]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> EndVideoHearingAsync(Guid conferenceId)
        {
            try
            {
                _logger.LogDebug("Attempting to end hearing");
                await _videoPlatformService.EndHearingAsync(conferenceId);
                return Accepted();
            }
            catch (KinlyApiException ex)
            {
                _logger.LogError(ex, "Error from Kinly API. Unable to end video hearing");
                return StatusCode(ex.StatusCode, ex.Response);
            }
        }
        /// <summary>	
        /// Request technical assistance. This will suspend a hearing.	
        /// </summary>	
        /// <param name="conferenceId">conference id</param>	
        /// <returns>No Content status</returns>	
        [HttpPost("{conferenceId}/suspend")]	
        [OpenApiOperation("SuspendHearing")]	
        [ProducesResponseType((int)HttpStatusCode.Accepted)]	
        public async Task<IActionResult> SuspendHearingAsync(Guid conferenceId)	
        {	
            try	
            {	
                await _videoPlatformService.SuspendHearingAsync(conferenceId);	
                return Accepted();	
            }	
            catch (KinlyApiException ex)	
            {	
                _logger.LogError(ex, "Unable to request technical assistance for video hearing");	
                return StatusCode(ex.StatusCode, ex.Response);	
            }	
        }
        
        /// <summary>
        /// Transfer a participant in or out of a hearing
        /// </summary>
        /// <param name="conferenceId">Id for conference</param>
        /// <param name="transferRequest">Participant and direction of transfer</param>
        /// <returns></returns>
        [HttpPost("{conferenceId}/transfer")]
        [OpenApiOperation("TransferParticipant")]
        [ProducesResponseType((int) HttpStatusCode.Accepted)]
        public async Task<IActionResult> TransferParticipantAsync(Guid conferenceId, TransferParticipantRequest transferRequest)
        {
            var participantId = transferRequest.ParticipantId?.ToString() ?? transferRequest.RoomId.ToString();
            var transferType = transferRequest.TransferType;
            try
            {
                switch (transferType)
                {
                    case TransferType.Call:
                        _logger.LogDebug("Attempting to transfer {Participant} into hearing room in {Conference}",
                            participantId, conferenceId);
                        await _videoPlatformService.TransferParticipantAsync(conferenceId, participantId,
                            RoomType.WaitingRoom.ToString(), RoomType.HearingRoom.ToString());
                        break;
                    case TransferType.Dismiss:
                        _logger.LogDebug("Attempting to transfer {Participant} out of hearing room in {Conference}",
                            participantId, conferenceId);
                        await _videoPlatformService.TransferParticipantAsync(conferenceId, participantId,
                            RoomType.HearingRoom.ToString(), RoomType.WaitingRoom.ToString());
                        break;
                    default:
                        _logger.LogWarning("Unable to transfer Participant {Participant} in {Conference}. Transfer type {TransferType} is unsupported",
                             participantId, conferenceId, transferType);
                        throw new InvalidOperationException($"Unsupported transfer type: {transferType}");
                }

                return Accepted();
            }
            catch (KinlyApiException ex)
            {
                _logger.LogError(ex,
                    "Error from Kinly API. Unable to {TransferType} Participant {Participant} in/from {Conference}",
                    transferType, participantId, conferenceId);
                return StatusCode(ex.StatusCode, ex.Response);
            }
        }
    }
}
