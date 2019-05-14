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
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using Task = VideoApi.Domain.Task;

namespace Video.API.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("conferences")]
    public class TasksController : ControllerBase
    {
        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;

        public TasksController(IQueryHandler queryHandler, ICommandHandler commandHandler)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
        }

        /// <summary>
        /// Get pending tasks
        /// </summary>
        /// <param name="conferenceId">The id of the conference to retrieve tasks from</param>
        /// <returns></returns>
        [HttpGet("{conferenceId}/tasks")]
        [SwaggerOperation(OperationId = "GetPendingTasks")]
        [ProducesResponseType(typeof(List<TaskResponse>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetPendingTasks(Guid conferenceId)
        {
            var query = new GetIncompleteTasksForConferenceQuery(conferenceId);
            try
            {
                var tasks = await _queryHandler.Handle<GetIncompleteTasksForConferenceQuery, List<Task>>(query);
                var mapper = new TaskToResponseMapper();
                var response = tasks.Select(mapper.MapTaskToResponse);
                return Ok(response);
            }
            catch (ConferenceNotFoundException)
            {
                return NotFound();
            }

        }

        /// <summary>
        /// Update existing tasks
        /// </summary>
        /// <param name="conferenceId">The id of the conference to update</param>
        /// <param name="taskId">The id of the task to update</param>
        /// <returns></returns>
        [HttpPatch("{conferenceId}/tasks/{taskid}")]
        [SwaggerOperation(OperationId = "UpdateTaskStatus")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateTaskStatus(Guid conferenceId, long taskId, [FromBody] UpdateTaskRequest updateTaskRequest)
        {
            var command = new UpdateTaskCommand(conferenceId, taskId, updateTaskRequest.UpdatedBy);
            try
            {
                await _commandHandler.Handle(command);
            }
            catch (TaskNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }

    }
}