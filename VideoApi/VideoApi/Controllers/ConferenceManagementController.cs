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
using VideoApi.Services;
using ConferenceRole = VideoApi.Contract.Enums.ConferenceRole;
using Endpoint = VideoApi.Domain.Endpoint;
using EndpointState = VideoApi.Domain.Enums.EndpointState;
using ParticipantState = VideoApi.Domain.Enums.ParticipantState;
using RoomType = VideoApi.Domain.Enums.RoomType;
using StartHearingRequest = VideoApi.Contract.Requests.StartHearingRequest;
using Supplier = VideoApi.Domain.Enums.Supplier;

namespace VideoApi.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [OpenApiTag("Conference Management")]
    [Route("conferences")]
    [ApiController]
    public class ConferenceManagementController(
        ISupplierPlatformServiceFactory supplierPlatformServiceFactory,
        ILogger<ConferenceManagementController> logger,
        IQueryHandler queryHandler)
        : ControllerBase
    {
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
            logger.LogDebug("Attempting to start hearing");
            var hearingLayout =
                HearingLayoutMapper.MapLayoutToVideoHearingLayout(
                    request.Layout.GetValueOrDefault(HearingLayout.Dynamic));

            var conference =
                await queryHandler.Handle<GetConferenceByIdQuery, Conference>(new GetConferenceByIdQuery(conferenceId));

            var participants = conference.Participants.Where(x =>
                x.State is ParticipantState.Available or ParticipantState.InConsultation
                && x.CanAutoTransferToHearingRoom() && !x.IsHost()).Select(x => x.Id.ToString()).ToList();

            var endpoints = conference.Endpoints
                .Where(x => x.State is EndpointState.Connected or EndpointState.InConsultation)
                .Select(x => x.Id.ToString()).ToList();

            var telephoneParticipants = conference.GetTelephoneParticipants()
                .Where(x => x.State is TelephoneState.Connected)
                .Select(x => x.Id.ToString()).ToList();

            var allIdsToTransfer = participants.Concat(endpoints).Concat(telephoneParticipants).ToList();
            // if only hosts are connected and no participants the supplier will not start the hearing, so provide the host id to force the hearing to start
            allIdsToTransfer.Add(request.TriggeredByHostId.ToString());

            var hosts = request.Hosts.Select(h => h.ToString()).ToList();
            var hostsForScreening = request.HostsForScreening.Select(h => h.ToString()).ToList();

            var videoPlatformService = supplierPlatformServiceFactory.Create(conference.Supplier);
            if (conference.Supplier == Supplier.Vodafone)
            {
                await videoPlatformService.StartHearingAsync(conferenceId, request.TriggeredByHostId.ToString(),
                    allIdsToTransfer, hosts, hearingLayout, request.MuteGuests ?? true, hostsForScreening);
            }
            else
            {
                await videoPlatformService.StartHearingAsync(conferenceId, request.TriggeredByHostId.ToString(),
                    allIdsToTransfer, hosts, hearingLayout, request.MuteGuests ?? false, hostsForScreening);
            }


            return Accepted();
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
            logger.LogDebug("Attempting to pause hearing");
            var conference =
                await queryHandler.Handle<GetConferenceByIdQuery, Conference>(new GetConferenceByIdQuery(conferenceId));
            var videoPlatformService = supplierPlatformServiceFactory.Create(conference.Supplier);
            await videoPlatformService.PauseHearingAsync(conferenceId);
            return Accepted();
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
            logger.LogDebug("Attempting to end hearing");
            var conference =
                await queryHandler.Handle<GetConferenceByIdQuery, Conference>(new GetConferenceByIdQuery(conferenceId));
            var videoPlatformService = supplierPlatformServiceFactory.Create(conference.Supplier);
            await videoPlatformService.EndHearingAsync(conferenceId);
            return Accepted();
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
            var conference =
                await queryHandler.Handle<GetConferenceByIdQuery, Conference>(new GetConferenceByIdQuery(conferenceId));
            var videoPlatformService = supplierPlatformServiceFactory.Create(conference.Supplier);
            await videoPlatformService.SuspendHearingAsync(conferenceId);
            return Accepted();
        }

        /// <summary>
        /// Transfer a participant or endpoint in or out of a hearing
        /// </summary>
        /// <param name="conferenceId">Id for conference</param>
        /// <param name="transferRequest">Participant or Endpoint ID and direction of transfer</param>
        /// <returns></returns>
        [HttpPost("{conferenceId}/transfer")]
        [OpenApiOperation("TransferParticipant")]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> TransferParticipantAsync(Guid conferenceId,
            TransferParticipantRequest transferRequest)
        {
            var conference =
                await queryHandler.Handle<GetConferenceByIdQuery, Conference>(
                    new GetConferenceByIdQuery(conferenceId));

            string supplierParticipantId = null;
            string transferFromRoomType = null;
            var participant = conference.GetParticipants().SingleOrDefault(x => x.Id == transferRequest.ParticipantId);
            var endpoint = conference.GetEndpoints().SingleOrDefault(x => x.Id == transferRequest.ParticipantId);
            var role = transferRequest.ConferenceRole == ConferenceRole.Guest
                ? Domain.Enums.ConferenceRole.Guest
                : Domain.Enums.ConferenceRole.Host;
            if (participant == null && endpoint == null)
            {
                return NotFound($"Id {transferRequest.ParticipantId} does not belong to a participant or endpoint");
            }
            
            if (participant != null)
            {
                supplierParticipantId = participant.GetParticipantRoom()?.Id.ToString() ?? participant.Id.ToString();
                transferFromRoomType = TransferFromRoomType(participant);
                // force role of host if participant role is judge or staff member to prevent accidental role change
                if (participant.IsHost())
                {
                    role = Domain.Enums.ConferenceRole.Host;
                }
            }
            
            if(endpoint != null)
            {
                supplierParticipantId = endpoint.Id.ToString();
                transferFromRoomType = TransferFromRoomType(endpoint);
            }

            var transferType = transferRequest.TransferType;
            var videoPlatformService = supplierPlatformServiceFactory.Create(conference.Supplier);

            switch (transferType)
            {
                case TransferType.Call:
                    logger.LogDebug("Attempting to transfer {Participant} into hearing room in {Conference}",
                        supplierParticipantId, conferenceId);
                    await videoPlatformService.TransferParticipantAsync(conferenceId, supplierParticipantId,
                        transferFromRoomType, RoomType.HearingRoom.ToString(), role);
                    break;
                case TransferType.Dismiss:
                    logger.LogDebug("Attempting to transfer {Participant} out of hearing room in {Conference}",
                        supplierParticipantId, conferenceId);
                    await videoPlatformService.TransferParticipantAsync(conferenceId, supplierParticipantId,
                        RoomType.HearingRoom.ToString(), RoomType.WaitingRoom.ToString(), role);
                    break;
                default:
                    logger.LogWarning(
                        "Unable to transfer Participant {Participant} in {Conference}. Transfer type {TransferType} is unsupported",
                        supplierParticipantId, conferenceId, transferType);
                    throw new InvalidOperationException($"Unsupported transfer type: {transferType}");
            }

            return Accepted();
        }

        private static string TransferFromRoomType(ParticipantBase participant)
        {
            return string.IsNullOrWhiteSpace(participant.CurrentConsultationRoom?.Label)
                ? RoomType.WaitingRoom.ToString()
                : participant.CurrentConsultationRoom.Label;
        }

        private static string TransferFromRoomType(Endpoint endpoint)
        {
            return string.IsNullOrWhiteSpace(endpoint.CurrentConsultationRoom?.Label)
                ? RoomType.WaitingRoom.ToString()
                : endpoint.CurrentConsultationRoom.Label;
        }
    }
}
