using System;
using System.Collections.Generic;
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
    public class AlertsController
    {
        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;

        public AlertsController(IQueryHandler queryHandler, ICommandHandler commandHandler)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
        }
        
        [HttpPut("{conferenceId}/alerts")]
        [SwaggerOperation(OperationId = "AddAlertToConference")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddAlertToConference(Guid conferenceId, AddAlertRequest request)
        {
            throw new NotImplementedException();
        }
        
        [HttpGet("{conferenceId}/alerts")]
        [SwaggerOperation(OperationId = "GetPendingAlerts")]
        [ProducesResponseType(typeof(List<AlertResponse>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetPendingAlerts(Guid conferenceId)
        {
            throw new NotImplementedException();
        }
    }
}