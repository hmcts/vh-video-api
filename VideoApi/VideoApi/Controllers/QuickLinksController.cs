using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VideoApi.Common.Logging;
using VideoApi.Common.Security;
using VideoApi.Contract.Consts;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.DTOs;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Extensions;
using VideoApi.Mappings;

namespace VideoApi.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("quickjoin")]
    public class QuickLinksController(
        ICommandHandler commandHandler,
        IQueryHandler queryHandler,
        IQuickLinksJwtTokenProvider quickLinksJwtTokenProvider,
        ILogger<QuickLinksController> logger)
        : ControllerBase
    {
        [HttpGet("ValidateQuickLink/{hearingId}")]
        [OpenApiOperation("ValidateQuickLink")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ValidateQuickLink(Guid hearingId)
        {
            try
            {
                var query = new GetConferenceByHearingRefIdQuery(hearingId);
                var conference =
                    await queryHandler.Handle<GetConferenceByHearingRefIdQuery, Conference>(query);

                var isQuickLinkValid = QuickLink.IsValid(conference);

                return Ok(isQuickLinkValid);
            }
            catch (ConferenceNotFoundException ex)
            {
                logger.LogUnableToFindConferenceError(ex);
                return NotFound(false);
            }
        }

        [HttpPost("AddQuickLinkParticipant/{hearingId}")]
        [OpenApiOperation("AddQuickLinkParticipant")]
        [ProducesResponseType(typeof(AddQuickLinkParticipantResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddQuickLinkParticipant(Guid hearingId, AddQuickLinkParticipantRequest quickLinkParticipantRequest)
        {
            try
            {
                var query = new GetConferenceByHearingRefIdQuery(hearingId);
                var conference =
                    await queryHandler.Handle<GetConferenceByHearingRefIdQuery, Conference>(query);

                var participant = new QuickLinkParticipant(quickLinkParticipantRequest.Name, quickLinkParticipantRequest.UserRole.MapToDomainEnum());

                var participantsToAdd = new List<ParticipantBase> { participant };

                var addParticipantsToConferenceCommand = new AddParticipantsToConferenceCommand(conference.Id, participantsToAdd, new List<LinkedParticipantDto>());

                await commandHandler.Handle(addParticipantsToConferenceCommand);

                var jwtDetails = quickLinksJwtTokenProvider.GenerateToken(participant.DisplayName, participant.Username,
                    participant.UserRole);

                var addQuickLinkParticipantTokenCommand = new AddQuickLinkParticipantTokenCommand(participant.Id, jwtDetails);

                await commandHandler.Handle(addQuickLinkParticipantTokenCommand);

                var response = new AddQuickLinkParticipantResponse
                {
                    Token = jwtDetails.Token,
                    ConferenceId = conference.Id,
                    Participant = ParticipantResponseMapper.Map(participant)
                };

                return Ok(response);
            }
            catch (ConferenceNotFoundException ex)
            {
                logger.LogConferenceNotFoundByHearingError(hearingId, ex);
                return NotFound(false);
            }
        }

        [HttpGet("GetQuickLinkParticipantByUserName/{userName}")]
        [OpenApiOperation("GetQuickLinkParticipantByUserName")]
        [ProducesResponseType(typeof(ParticipantResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetQuickLinkParticipantByUserName(string userName)
        {

            var query = new GetQuickLinkParticipantByIdQuery(Guid.Parse(userName.Replace(QuickLinkParticipantConst.Domain, string.Empty)));
            var quickLinkParticipant = await queryHandler.Handle<GetQuickLinkParticipantByIdQuery, ParticipantBase>(query);

            if (quickLinkParticipant == null)
            {
                logger.LogQuickLinkNotFoundByUserNameError(userName);
                return NotFound();
            }

            return Ok(new ParticipantResponse
            {
                Id = quickLinkParticipant.Id,
                Username = quickLinkParticipant.Id.ToString(),
                DisplayName = quickLinkParticipant.DisplayName,
                UserRole = (UserRole)quickLinkParticipant.UserRole
            });
        }
    }
}
