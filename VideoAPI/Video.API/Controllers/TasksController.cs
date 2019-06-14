using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<TasksController> _logger;

        public TasksController(IQueryHandler queryHandler, ICommandHandler commandHandler, ILogger<TasksController> logger)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
            _logger = logger;
        }

        /// <summary>
        /// Get tasks for a conference
        /// </summary>
        /// <param name="conferenceId">The id of the conference to retrieve tasks from</param>
        /// <returns></returns>
        [HttpGet("{conferenceId}/tasks")]
        [SwaggerOperation(OperationId = "GetTasksForConference")]
        [ProducesResponseType(typeof(List<TaskResponse>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetTasksForConference(Guid conferenceId)
        {
            _logger.LogDebug("GetTasksForConference");
            var query = new GetTasksForConferenceQuery(conferenceId);
            try
            {
                var tasks = await _queryHandler.Handle<GetTasksForConferenceQuery, List<Task>>(query);
                var mapper = new TaskToResponseMapper();
                var response = tasks.Select(mapper.MapTaskToResponse);
                return Ok(response);
            }
            catch (ConferenceNotFoundException)
            {
                _logger.LogError($"Unable to find conference {conferenceId}");
                return NotFound();
            }
        }

        /// <summary>
        /// Update existing tasks
        /// </summary>
        /// <param name="conferenceId">The id of the conference to update</param>
        /// <param name="taskId">The id of the task to update</param>
        /// <returns></returns>
        [HttpPatch("{conferenceId}/tasks/{taskId}")]
        [SwaggerOperation(OperationId = "UpdateTaskStatus")]
        [ProducesResponseType(typeof(TaskResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateTaskStatus(Guid conferenceId, long taskId,
            [FromBody] UpdateTaskRequest updateTaskRequest)
        {
            _logger.LogDebug("UpdateTaskStatus");
            var command = new UpdateTaskCommand(conferenceId, taskId, updateTaskRequest.UpdatedBy);
            
            try
            {
                await _commandHandler.Handle(command);
                var query = new GetTasksForConferenceQuery(conferenceId);
                var tasks = await _queryHandler.Handle<GetTasksForConferenceQuery, List<Task>>(query);
                _logger.LogInformation(
                    $"Completed task {taskId} in conference {conferenceId} by {updateTaskRequest.UpdatedBy}");
                var task = tasks.Single(x => x.Id == taskId);
                var response = new TaskToResponseMapper().MapTaskToResponse(task);
                return Ok(response);
            }
            catch (TaskNotFoundException)
            {
                _logger.LogError($"Unable to find task {taskId} in conference {conferenceId}");
                return NotFound();
            }
        }
    }
}