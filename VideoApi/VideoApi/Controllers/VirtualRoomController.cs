using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Mappings;
using VideoApi.Services.Contracts;
using VideoApi.Validations.Models;

namespace VideoApi.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("conferences")]
    [ApiController]
    public class VirtualRoomController : ControllerBase
    {
        private readonly IVirtualRoomService _roomService;
        private readonly IQueryHandler _queryHandler;
        private readonly ILogger<VirtualRoomController> _logger;

        public VirtualRoomController(IVirtualRoomService roomService, IQueryHandler queryHandler,
            ILogger<VirtualRoomController> logger)
        {
            _roomService = roomService;
            _logger = logger;
            _queryHandler = queryHandler;
        }

        /// <summary>
        /// Get a civilian VMR or return an existing one for a participant
        /// </summary>
        /// <param name="conferenceId"></param>
        /// <param name="participantId"></param>
        /// <returns></returns>
        [HttpGet("{conferenceId}/rooms/interpreter/{participantId}")]
        [OpenApiOperation("GetInterpreterRoomForParticipant")]
        [ProducesResponseType(typeof(SharedParticipantRoomResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetInterpreterRoomForParticipant(Guid conferenceId, Guid participantId)
        {
            var validation = await ValidateConferenceAndParticipant(conferenceId, participantId);
            if (validation.FailedResult != null)
            {
                return validation.FailedResult;
            }
            
            var response = await GetVmr(validation.Conference,validation.Participant, VirtualCourtRoomType.Civilian);
            return Ok(response);
        }
        
        /// <summary>
        /// Get a witness VMR or return an existing one for a participant
        /// </summary>
        /// <param name="conferenceId"></param>
        /// <param name="participantId"></param>
        /// <returns></returns>
        [HttpGet("{conferenceId}/rooms/witness/{participantId}")]
        [OpenApiOperation("GetWitnessRoomForParticipant")]
        [ProducesResponseType(typeof(SharedParticipantRoomResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetWitnessRoomForParticipant(Guid conferenceId, Guid participantId)
        {
            var validation = await ValidateConferenceAndParticipant(conferenceId, participantId);
            if (validation.FailedResult != null)
            {
                return validation.FailedResult;
            }
            
            var response = await GetVmr(validation.Conference,validation.Participant, VirtualCourtRoomType.Witness);
            return Ok(response);
        }
        
        /// <summary>
        /// Get a judicial office holder VMR or return an existing one for a participant
        /// </summary>
        /// <param name="conferenceId"></param>
        /// <param name="participantId"></param>
        /// <returns></returns>
        [HttpGet("{conferenceId}/rooms/judicial/{participantId}")]
        [OpenApiOperation("GetJudicialRoomForParticipant")]
        [ProducesResponseType(typeof(SharedParticipantRoomResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetJudicialRoomForParticipant(Guid conferenceId, Guid participantId)
        {
            var validation = await ValidateConferenceAndParticipant(conferenceId, participantId);
            if (validation.FailedResult != null)
            {
                return validation.FailedResult;
            }
            
            var response = await GetVmr(validation.Conference,validation.Participant, VirtualCourtRoomType.JudicialShared);
            return Ok(response);
        }

        /*
        /// <summary>
        /// Get conferences Hearing rooms
        /// </summary>
        /// <returns>Hearing rooms details</returns>
        [AllowAnonymous]
        [HttpGet("dateStamp/interpreterRooms")]
        [OpenApiOperation("GetConferencesInterpreterRooms")]
        [ProducesResponseType(typeof(List<SharedParticipantRoomResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NoContent)]
        public async Task<IActionResult> GetConferencesInterpreterRoomsAsync([FromQuery]string dateStamp)
        {
            _logger.LogDebug("GetConferencesInterpreterRooms");

            try
            {
                var date = DateTime.ParseExact(dateStamp, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var conferences =
                    await _queryHandler.Handle<GetConferenceInterpreterRoomsByDateQuery, List<Conference>>(
                        new GetConferenceInterpreterRoomsByDateQuery(date));

                var response = ConferenceInterpreterRoomsResponseMapper.Map(conferences);

                return Ok(response);
            }
            catch (FormatException e)
            {
                _logger.LogError(e, e.Message);
                return NoContent();
            }
        }
        */

        private async Task<ConferenceParticipantExists> ValidateConferenceAndParticipant(Guid conferenceId,
            Guid participantId)
        {
            var conference =
                await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(
                    new GetConferenceByIdQuery(conferenceId));

            var validation = new ConferenceParticipantExists();
            if (conference == null)
            {
                _logger.LogWarning(
                    "Failed to get a room for conference {Conference} because it does not exist",
                    conferenceId);
                validation.FailedResult = NotFound("Conference does not exist");
                return validation;
            }

            var participant = conference.GetParticipants().SingleOrDefault(x => x.Id == participantId);
            if (participant == null)
            {
                _logger.LogWarning(
                    "Failed to get an room for conference {Conference} because {Participant} does not exist",
                    conferenceId, participantId);
                validation.FailedResult = NotFound("Participant does not exist");
                return validation;
            }

            validation.Conference = conference;
            validation.Participant = participant;
            return validation;
        }

        private async Task<SharedParticipantRoomResponse> GetVmr(Conference conference, ParticipantBase participant, VirtualCourtRoomType roomType)
        {
            ParticipantRoom participantRoom;
            switch (roomType)
            {
                case VirtualCourtRoomType.Witness:
                    participantRoom = await _roomService.GetOrCreateAWitnessVirtualRoom(conference, participant);
                    break;
                case VirtualCourtRoomType.JudicialShared:
                    participantRoom = await _roomService.GetOrCreateAJudicialVirtualRoom(conference, participant);
                    break;
                default:
                    participantRoom = await _roomService.GetOrCreateAnInterpreterVirtualRoom(conference, participant);
                    break;
            }
            
            _logger.LogDebug(
                "Returning participant room {Room} for participant {Participant} in conference {Conference}", participantRoom.Id,
                participant.Id, conference.Id);
            return SharedParticipantRoomResponseMapper.MapRoomToResponse(participantRoom);
        }
    }
}
