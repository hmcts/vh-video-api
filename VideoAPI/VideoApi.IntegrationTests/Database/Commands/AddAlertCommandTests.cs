using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;
using TaskStatus = VideoApi.Domain.Enums.TaskStatus;

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class AddAlertCommandTests : DatabaseTestsBase
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
        public async Task should_add_an_alert(TaskType taskType)
        {
            var seededConference = await TestDataManager.SeedConference();
            _newConferenceId = seededConference.Id;
            const string body = "Automated Test Add Task";

            var command = new AddTaskCommand(_newConferenceId, body, taskType);
            await _handler.Handle(command);

            Conference conference;
            using (var db = new VideoApiDbContext(VideoBookingsDbContextOptions))
            {
                conference = await db.Conferences.Include(x => x.Tasks)
                    .SingleAsync(x => x.Id == command.ConferenceId);
            }

            var savedAlert = conference.GetTasks().First(x => x.Body == body && x.Type == taskType);

            savedAlert.Should().NotBeNull();
            savedAlert.Status.Should().Be(TaskStatus.ToDo);
            savedAlert.Updated.Should().BeNull();
            savedAlert.UpdatedBy.Should().BeNull();
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