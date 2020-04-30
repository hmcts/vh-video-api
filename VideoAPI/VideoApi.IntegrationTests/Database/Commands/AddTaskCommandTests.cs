using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;
using TaskStatus = VideoApi.Domain.Enums.TaskStatus;

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class AddTaskCommandTests : DatabaseTestsBase
    {
        private AddTaskCommandHandler _handler;
        private Guid _newConferenceId;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new AddTaskCommandHandler(context);
            _newConferenceId = Guid.Empty;
        }

        [TestCase(TaskType.Judge)]
        [TestCase(TaskType.Hearing)]
        [TestCase(TaskType.Participant)]
        public async Task Should_add_an_task(TaskType taskType)
        {
            var seededConference = await TestDataManager.SeedConference();
            _newConferenceId = seededConference.Id;
            const string body = "Automated Test Add Task";

            var command = CreateTaskCommand(seededConference, body, taskType);
            await _handler.Handle(command);

            List<Domain.Task> tasks;
            await using (var db = new VideoApiDbContext(VideoBookingsDbContextOptions))
            {
                tasks = await db.Tasks.Where(x => x.ConferenceId == command.ConferenceId).ToListAsync();
            }

            var savedAlert = tasks.First(x => x.Body == body && x.Type == taskType);

            savedAlert.Should().NotBeNull();
            savedAlert.Status.Should().Be(TaskStatus.ToDo);
            savedAlert.Updated.Should().BeNull();
            savedAlert.UpdatedBy.Should().BeNull();
            savedAlert.ConferenceId.Should().Be(command.ConferenceId);
        }

        private AddTaskCommand CreateTaskCommand(Conference conference, string body, TaskType taskType)
        {
            if (taskType == TaskType.Hearing)
            {
                return new AddTaskCommand(conference.Id, conference.Id, body, taskType);
            }

            if (taskType == TaskType.Judge)
            {
                var participant = conference.GetParticipants().First(x => x.UserRole == UserRole.Judge);
                return new AddTaskCommand(conference.Id, participant.Id, body, taskType);
            }
            else
            {
                var participant = conference.GetParticipants().First(x => x.UserRole == UserRole.Individual);
                return new AddTaskCommand(conference.Id, participant.Id, body, taskType);
            }
        }

              
        [TearDown]
        public async Task TearDown()
        {
            if (_newConferenceId != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test conference {_newConferenceId}");
                await TestDataManager.RemoveConference(_newConferenceId);
            }
        }
    }
}
