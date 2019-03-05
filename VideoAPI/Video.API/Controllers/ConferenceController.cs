using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;

namespace Video.API.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("conferences")]
    [ApiController]
    public class ConferenceController : ControllerBase
    {
        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;

        public ConferenceController(IQueryHandler queryHandler, ICommandHandler commandHandler)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
        }

        /// <summary>
        /// Request to book a conference
        /// </summary>
        /// <param name="request">Details of a conference</param>
        /// <returns>Details of the new conference</returns>
        [HttpPost]
        [SwaggerOperation(OperationId = "BookNewConference")]
        [ProducesResponseType(typeof(ConferenceDetailsResponse), (int) HttpStatusCode.Created)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> BookNewConference(BookNewConferenceRequest request)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Update the conference status
        /// </summary>
        /// <param name="conferenceId">The id of the conference to update</param>
        /// <param name="request">New status for the conference</param>
        /// <returns></returns>
        [HttpPatch("{conferenceId}")]
        [SwaggerOperation(OperationId = "UpdateConferenceStatus")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateConferenceStatus(Guid conferenceId,
            UpdateConferenceStatusRequest request)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the details of a conference
        /// </summary>
        /// <param name="conferenceId">Id of the conference</param>
        /// <returns>Full details including participants and statuses of a conference</returns>
        [HttpGet("{conferenceId}")]
        [SwaggerOperation(OperationId = "GetConferenceDetailsById")]
        [ProducesResponseType(typeof(ConferenceDetailsResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetConferenceDetailsById(Guid conferenceId)
        {
            throw new NotImplementedException();
        }
    }
}