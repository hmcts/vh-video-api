using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Video.API.Extensions;
using Video.API.Validations;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;

namespace Video.API.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("conferences")]
    [ApiController]
    public class ParticipantsController : ControllerBase
    {
        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;

        public ParticipantsController(ICommandHandler commandHandler, IQueryHandler queryHandler)
        {
            _commandHandler = commandHandler;
            _queryHandler = queryHandler;
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
            if (conferenceId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(conferenceId), $"Please provide a valid {nameof(conferenceId)}");
                return BadRequest(ModelState);
            }

            if (participantId <= 0)
            {
                ModelState.AddModelError(nameof(participantId), $"Please provide a valid {nameof(participantId)}");
                return BadRequest(ModelState);
            }

            var result = await new UpdateParticipantStatusRequestValidation().ValidateAsync(request);
            if (!result.IsValid)
            {
                ModelState.AddFluentValidationErrors(result.Errors);
                return BadRequest(ModelState);
            }

            var getConferenceByIdQuery = new GetConferenceByIdQuery(conferenceId);
            var queriedConference =
                await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);

            if (queriedConference == null)
            {
                return NotFound();
            }

            var participant = queriedConference.GetParticipants().SingleOrDefault(x => x.Id == participantId);
            if (participant == null)
            {
                return NotFound();
            }

            var command = new UpdateParticipantStatusCommand(conferenceId, participantId, request.State);
            await _commandHandler.Handle(command);
            return NoContent();
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
            if (conferenceId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(conferenceId), $"Please provide a valid {nameof(conferenceId)}");
                return BadRequest(ModelState);
            }

            var result = await new AddParticipantsToConferenceRequestValidation().ValidateAsync(request);
            if (!result.IsValid)
            {
                ModelState.AddFluentValidationErrors(result.Errors);
                return BadRequest(ModelState);
            }

            var getConferenceByIdQuery = new GetConferenceByIdQuery(conferenceId);
            var queriedConference =
                await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);

            if (queriedConference == null)
            {
                return NotFound();
            }

            var participants = request.Participants.Select(x =>
                    new Participant(x.ParticipantRefId, x.Name, x.DisplayName, x.Username, x.HearingRole,
                        x.CaseTypeGroup))
                .ToList();

            var addParticipantCommand = new AddParticipantsToConferenceCommand(conferenceId, participants);
            await _commandHandler.Handle(addParticipantCommand);

            return NoContent();
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
            if (conferenceId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(conferenceId), $"Please provide a valid {nameof(conferenceId)}");
                return BadRequest(ModelState);
            }

            if (participantId <= 0)
            {
                ModelState.AddModelError(nameof(participantId), $"Please provide a valid {nameof(participantId)}");
                return BadRequest(ModelState);
            }

            var getConferenceByIdQuery = new GetConferenceByIdQuery(conferenceId);
            var queriedConference =
                await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);

            if (queriedConference == null)
            {
                return NotFound();
            }

            var participant = queriedConference.GetParticipants().SingleOrDefault(x => x.Id == participantId);
            if (participant == null)
            {
                return NotFound();
            }

            var participants = new List<Participant> {participant};
            var command = new RemoveParticipantsFromConferenceCommand(conferenceId, participants);
            await _commandHandler.Handle(command);
            return NoContent();
        }
    }
}