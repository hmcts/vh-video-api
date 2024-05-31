using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Mappings;
using VideoApi.Services.Contracts;
using VideoApi.Services.Clients;
using StartHearingRequest = VideoApi.Contract.Requests.StartHearingRequest;

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
        private readonly IQueryHandler _queryHandler;
        private readonly ILogger<ConferenceManagementController> _logger;

        public ConferenceManagementController(IVideoPlatformService videoPlatformService,
            ILogger<ConferenceManagementController> logger, IQueryHandler queryHandler)
        {
            _videoPlatformService = videoPlatformService;
            _logger = logger;
            _queryHandler = queryHandler;
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
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> StartVideoHearingAsync(Guid conferenceId, StartHearingRequest request)
        {
            try
            {
                _logger.LogDebug("Attempting to start hearing");
                var hearingLayout =
                    HearingLayoutMapper.MapLayoutToVideoHearingLayout(
                        request.Layout.GetValueOrDefault(HearingLayout.Dynamic));
               
                // ***Remove this line when video web sends the triggered by host id attribute ****ALSO, do not expose ParticipantsToForceTransfer sa this is populated here
                var triggeredByHostId = request.TriggeredByHostId ?? request.ParticipantsToForceTransfer?.Single();

                var conference = await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(new GetConferenceByIdQuery(conferenceId));
                var participants = conference.Participants.Where(x =>
                    (x.State == ParticipantState.Available || x.State == ParticipantState.InConsultation)
                    && x.UserRole != UserRole.QuickLinkParticipant && x.UserRole != UserRole.QuickLinkObserver &&
                    x.HearingRole != "Witness");
                
                // all endpoints get pulled into a hearing
                var endpoints = conference.Endpoints.Where(x =>
                    x.State == EndpointState.Connected || x.State == EndpointState.InConsultation);
                
                var ids = participants.Select(x => x.Id.ToString()).ToList();
                ids.AddRange(endpoints.Select(x => x.Id.ToString()));

                await _videoPlatformService.StartHearingAsync(conferenceId, triggeredByHostId, ids, hearingLayout, request.MuteGuests ?? true);
           
                return Accepted();
            }
            catch (SupplierApiException ex)
            {
                if (ex.StatusCode == (int)HttpStatusCode.BadRequest)
                {
                    return BadRequest(
                        $"Invalid list of participants provided for {nameof(request.ParticipantsToForceTransfer)}. {request.ParticipantsToForceTransfer}");
                }
                
                _logger.LogError(ex, "Error from supplier API. Unable to start video hearing");
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
            catch (SupplierApiException ex)
            {
                _logger.LogError(ex, "Error from supplier API. Unable to pause video hearing");
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
            catch (SupplierApiException ex)
            {
                _logger.LogError(ex, "Error from supplier API. Unable to end video hearing");
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
            catch (SupplierApiException ex)	
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
            var conference =
                await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(
                    new GetConferenceByIdQuery(conferenceId));
            var participant = conference.GetParticipants().Single(x => x.Id == transferRequest.ParticipantId);
            var supplierParticipantId = participant.GetParticipantRoom()?.Id.ToString() ?? participant.Id.ToString();
            var transferType = transferRequest.TransferType;
            try
            {
                switch (transferType)
                {
                    case TransferType.Call:
                        _logger.LogDebug("Attempting to transfer {Participant} into hearing room in {Conference}",
                            supplierParticipantId, conferenceId);
                        await _videoPlatformService.TransferParticipantAsync(conferenceId, supplierParticipantId,
                           TransferFromRoomType(participant), RoomType.HearingRoom.ToString());
                        break;
                    case TransferType.Dismiss:
                        _logger.LogDebug("Attempting to transfer {Participant} out of hearing room in {Conference}",
                            supplierParticipantId, conferenceId);
                        await _videoPlatformService.TransferParticipantAsync(conferenceId, supplierParticipantId,
                            RoomType.HearingRoom.ToString(), RoomType.WaitingRoom.ToString());
                        break;
                    default:
                        _logger.LogWarning("Unable to transfer Participant {Participant} in {Conference}. Transfer type {TransferType} is unsupported",
                            supplierParticipantId, conferenceId, transferType);
                        throw new InvalidOperationException($"Unsupported transfer type: {transferType}");
                }

                return Accepted();
            }
            catch (SupplierApiException ex)
            {
                _logger.LogError(ex,
                    "Error from supplier API. Unable to {TransferType} Participant {Participant} in/from {Conference}",
                    transferType, supplierParticipantId, conferenceId);
                return StatusCode(ex.StatusCode, ex.Response);
            }
        }

        private string TransferFromRoomType(ParticipantBase participant)
        {
            return string.IsNullOrWhiteSpace(participant.CurrentConsultationRoom?.Label) ? RoomType.WaitingRoom.ToString() : participant.CurrentConsultationRoom.Label;
        }
    }
}
