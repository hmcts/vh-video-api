using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using VideoApi.Contract.Requests;
using VideoApi.Controllers;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Tasks
{
    public class TasksControllerTests
    {
        private Mock<IQueryHandler> queryHandler;
        private Mock<ICommandHandler> commandHandler;
        private Mock<ILogger<TasksController>> logger;
        private TasksController tasksController;

        [SetUp]
        public void TestsInitialize()
        {
            queryHandler = new Mock<IQueryHandler>();
            commandHandler = new Mock<ICommandHandler>();
            logger = new Mock<ILogger<TasksController>>();
            var task = new VideoApi.Domain.Task(Guid.NewGuid(),Guid.NewGuid(), "Test body", VideoApi.Domain.Enums.TaskType.Hearing);
            

            queryHandler
                .Setup(x => x.Handle<GetTasksForConferenceQuery, List<VideoApi.Domain.Task>>(It.IsAny<GetTasksForConferenceQuery>()))
                .ReturnsAsync(new List<VideoApi.Domain.Task> { task });

            tasksController = new TasksController(queryHandler.Object, commandHandler.Object, logger.Object);
        }

        [Test]
        public async Task Should_return_ok_result_on_updating_task_request()
        {
            var request = new UpdateTaskRequest
            {
                UpdatedBy = "Test Updated"
            };

            var result = await tasksController.UpdateTaskStatusAsync(Guid.NewGuid(),0, request);

            var typedResult = (OkObjectResult)result;
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            commandHandler.Verify(c => c.Handle(It.IsAny<UpdateTaskCommand>()), Times.Once);
        }

        [Test]
        public async Task Should_return_not_found_when_no_task_is_found()
        {
            var request = new UpdateTaskRequest
            {
                UpdatedBy = "Test Updated"
            };

            var result = await tasksController.UpdateTaskStatusAsync(Guid.NewGuid(), 10, request);

            var typedResult = (NotFoundResult)result;
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            commandHandler.Verify(c => c.Handle(It.IsAny<UpdateTaskCommand>()), Times.Once);
        }
    }
}
