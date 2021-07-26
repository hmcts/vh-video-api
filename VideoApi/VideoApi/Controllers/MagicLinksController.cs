using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.DTOs;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Extensions;

namespace VideoApi.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("quickjoin")]
    public class MagicLinksController : Controller
    {
        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;
        private readonly ILogger<MagicLinksController> _logger;

        public MagicLinksController(ICommandHandler commandHandler, IQueryHandler queryHandler, ILogger<MagicLinksController> logger)
        {
            _commandHandler = commandHandler;
            _queryHandler = queryHandler;
            _logger = logger;

        }

        [HttpGet("ValidateMagicLink/{hearingId}")]
        [AllowAnonymous]
        [OpenApiOperation("ValidateMagicLink")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ValidateMagicLink(Guid hearingId)
        {
            try
            {
                var query = new GetConferenceByHearingRefIdQuery(hearingId);
                var conference =
                    await _queryHandler.Handle<GetConferenceByHearingRefIdQuery, Conference>(query);

                if (conference == null || conference.IsClosed())
                    return Ok(false);
                return Ok(true);
            }
            catch (ConferenceNotFoundException ex)
            {
                _logger.LogError(ex, "Unable to find conference");
                return NotFound(false);
            }
        }

        [HttpPost("AddMagicLinkParticipant/{hearingId}")]
        [AllowAnonymous]
        [OpenApiOperation("ValidateMagicLink")]
        public async Task<IActionResult> AddMagicLinkParticipant(Guid hearingId, AddMagicLinkParticipantRequest magicLinkParticipantRequest)
        {
            try
            {
                var query = new GetConferenceByHearingRefIdQuery(hearingId);
                var conference =
                    await _queryHandler.Handle<GetConferenceByHearingRefIdQuery, Conference>(query);

                var participant = new MagicLinkParticipant(magicLinkParticipantRequest.Name, magicLinkParticipantRequest.UserRole.MapToDomainEnum());

                var participantsToAdd = new List<ParticipantBase> { participant };

                var command = new AddParticipantsToConferenceCommand(conference.Id, participantsToAdd, new List<LinkedParticipantDto>());

                await _commandHandler.Handle(command);

                return Ok();
            }
            catch (ConferenceNotFoundException ex)
            {
                _logger.LogError(ex, "Unable to find conference");
                return NotFound(false);
            }
        }
    }
}
