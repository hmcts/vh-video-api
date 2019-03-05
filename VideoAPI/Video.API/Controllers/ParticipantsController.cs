using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands.Core;

namespace Video.API.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("conferences")]
    [ApiController]
    public class ParticipantsController : ControllerBase
    {
        private readonly ICommandHandler _commandHandler;

        public ParticipantsController(ICommandHandler commandHandler)
        {
            _commandHandler = commandHandler;
        }

        /// <summary>
        /// Update the participant status
        /// </summary>
        /// <param name="conferenceId">The id of the conference participant belongs to</param>
        /// <param name="participantId">The id of the participant to update</param>
        /// <param name="request">New status for participant</param>
        /// <returns></returns>
        [HttpPatch("{conferenceId}/participants/{participantId}")]
        [SwaggerOperation(OperationId = "UpdateParticipantStatus")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateParticipantStatus(Guid conferenceId, long participantId,
            UpdateParticipantStatusRequest request)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add participants to a hearing
        /// </summary>
        /// <param name="conferenceId">The id of the conference to add participants to</param>
        /// <param name="request">Details of the participant</param>
        /// <returns></returns>
        [HttpPut("{conferenceId}/participants")]
        [SwaggerOperation(OperationId = "AddParticipantsToConference")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddParticipantsToConference(Guid conferenceId,
            AddParticipantsToConferenceRequest request)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Remove participants from a hearing
        /// </summary>
        /// <param name="conferenceId">The id of the conference to remove participants from</param>
        /// <param name="participantId">The id of the participant to remove</param>
        /// <returns></returns>
        [HttpDelete("{conferenceId}/participants/{participantId}")]
        [SwaggerOperation(OperationId = "RemoveParticipantFromConference")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RemoveParticipantFromConference(Guid conferenceId, long participantId)
        {
            throw new NotImplementedException();
        }
    }
}