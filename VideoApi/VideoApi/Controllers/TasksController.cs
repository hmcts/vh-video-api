using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Extensions;
using VideoApi.Mappings;
using Task = VideoApi.Domain.Task;
using VideoApi.Common.Logging;

namespace VideoApi.Controllers
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
        [OpenApiOperation("GetTasksForConference")]
        [ProducesResponseType(typeof(List<TaskResponse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetTasksForConferenceAsync(Guid conferenceId)
        {
            _logger.LogGetTasksForConference();
            var query = new GetTasksForConferenceQuery(conferenceId);
            try
            {
                var tasks = await _queryHandler.Handle<GetTasksForConferenceQuery, List<Task>>(query);
                var response = tasks.Select(TaskToResponseMapper.MapTaskToResponse);
                return Ok(response);
            }
            catch (Exception e)
            {
                _logger.LogUnableToFindTask(e);
                return BadRequest();
            }
        }

        /// <summary>
        /// Update existing tasks
        /// </summary>
        /// <param name="conferenceId">The id of the conference to update</param>
        /// <param name="taskId">The id of the task to update</param>
        /// <param name="updateTaskRequest">username of who completed the task</param>
        /// <returns></returns>
        [HttpPatch("{conferenceId}/tasks/{taskId}")]
        [OpenApiOperation("UpdateTaskStatus")]
        [ProducesResponseType(typeof(TaskResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateTaskStatusAsync(Guid conferenceId, long taskId,
            [FromBody] UpdateTaskRequest updateTaskRequest)
        {
            _logger.LogUpdateTaskStatus();
            try
            {
                var command = new UpdateTaskCommand(conferenceId, taskId, updateTaskRequest.UpdatedBy);
                await _commandHandler.Handle(command);
            }
            catch (TaskNotFoundException ex)
            {
                _logger.LogUnableToFindTask(ex);
                return NotFound();
            }

            var query = new GetTasksForConferenceQuery(conferenceId);
            var tasks = await _queryHandler.Handle<GetTasksForConferenceQuery, List<Task>>(query);
            var task = tasks.SingleOrDefault(x => x.Id == taskId);
            if (task == null)
            {
                _logger.LogUnableToFindTaskId(taskId);
                return NotFound();
            }
            var response = TaskToResponseMapper.MapTaskToResponse(task);
            return Ok(response);
        }

        /// <summary>
        /// Adds a task (alert) for a participant in the conference.
        /// </summary>
        /// <param name="conferenceId">The conference Id to add the task</param>
        /// <param name="addTaskRequest">The task request containing the task type and the alert (task name)</param>
        /// <returns></returns>
        [HttpPost("{conferenceId}/task")]
        [OpenApiOperation("AddTask")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> AddTaskAsync(Guid conferenceId, [FromBody] AddTaskRequest addTaskRequest)
        {
            _logger.LogAddingTask(addTaskRequest.Body, addTaskRequest.ParticipantId, conferenceId);
            try
            {
                var command = new AddTaskCommand(conferenceId, addTaskRequest.ParticipantId, addTaskRequest.Body, addTaskRequest.TaskType.MapToDomainEnum());
                await _commandHandler.Handle(command);
                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogUnableToAddTask(addTaskRequest.Body, addTaskRequest.ParticipantId, conferenceId, e);
                return BadRequest();
            }
        }
    }
}
