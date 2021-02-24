using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Mappings;
using VideoApi.Services.Contracts;

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
        /// Get a VMR or return an existing one for a participant
        /// </summary>
        /// <param name="conferenceId"></param>
        /// <param name="participantId"></param>
        /// <returns></returns>
        [HttpGet("{conferenceId}/rooms/interpreter/{participantId}")]
        [OpenApiOperation("GetInterpreterRoomForParticipant")]
        [ProducesResponseType(typeof(InterpreterRoomResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetInterpreterRoomForParticipant(Guid conferenceId, Guid participantId)
        {
            var conference =
                await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(
                    new GetConferenceByIdQuery(conferenceId));

            if (conference == null)
            {
                _logger.LogWarning(
                    "Failed to get an interpreter room for conference {Conference} because it does not exist",
                    conferenceId);
                return NotFound("Conference does not exist");
            }
            
            var participant = conference.GetParticipants().SingleOrDefault(x => x.Id == participantId);
            if (participant == null)
            {
                _logger.LogWarning(
                    "Failed to get an interpreter room for conference {Conference} because {Participant} does not exist",
                    conferenceId, participantId);
                return NotFound("Participant does not exist");
            }
            var room = await _roomService.GetOrCreateAnInterpreterVirtualRoom(conference, participant);
            _logger.LogDebug(
                "Returning interpreter room {Room} for participant {Participant} in conference {Conference}", room.Id,
                participant.Id, conference.Id);
            var response = InterpreterRoomResponseMapper.MapRoomToResponse(room);
            return Ok(response);
        }

    }
}
