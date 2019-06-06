using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Video.API.Mappings;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Services;

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
        private readonly IVideoPlatformService _videoPlatformService;

        public ParticipantsController(ICommandHandler commandHandler, IQueryHandler queryHandler,
            IVideoPlatformService videoPlatformService)
        {
            _commandHandler = commandHandler;
            _queryHandler = queryHandler;
            _videoPlatformService = videoPlatformService;
        }

        /// <summary>
        /// Add participants to a conference
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
            var getConferenceByIdQuery = new GetConferenceByIdQuery(conferenceId);
            var queriedConference =
                await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);

            if (queriedConference == null)
            {
                return NotFound();
            }

            var participants = request.Participants.Select(x =>
                    new Participant(x.ParticipantRefId, x.Name.Trim(), x.DisplayName.Trim(),
                        x.Username.ToLowerInvariant().Trim(), x.UserRole,
                        x.CaseTypeGroup)
                    {
                        Representee = x.Representee
                    })
                .ToList();

            var addParticipantCommand = new AddParticipantsToConferenceCommand(conferenceId, participants);
            await _commandHandler.Handle(addParticipantCommand);

            return NoContent();
        }

        /// <summary>
        /// Remove participants from a conference
        /// </summary>
        /// <param name="conferenceId">The id of the conference to remove participants from</param>
        /// <param name="participantId">The id of the participant to remove</param>
        /// <returns></returns>
        [HttpDelete("{conferenceId}/participants/{participantId}")]
        [SwaggerOperation(OperationId = "RemoveParticipantFromConference")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RemoveParticipantFromConference(Guid conferenceId, Guid participantId)
        {
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

        [HttpGet("{conferenceId}/participants/{participantId}/selftestresult")]
        [SwaggerOperation(OperationId = "GetTestCallResultForParticipant")]
        [ProducesResponseType(typeof(TestCallScoreResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetTestCallResultForParticipant(Guid conferenceId, Guid participantId)
        {
            // Verify that the participant exists????
            var testCallResult = await _videoPlatformService.GetTestCallScoreAsync(participantId);
            if (testCallResult == null)
            {
                return NotFound();
            }

            var command = new UpdateSelfTestCallResultCommand(conferenceId, participantId, testCallResult.Passed,
                testCallResult.Score);
            await _commandHandler.Handle(command);
            var response = new TaskCallResultResponseMapper().MapTaskToResponse(testCallResult);
            return Ok(response);
        }
    }
}