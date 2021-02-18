using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain.Enums;
using Alert = VideoApi.Domain.Task;
using Task = System.Threading.Tasks.Task;
using TaskStatus = VideoApi.Domain.Enums.TaskStatus;

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class UpdateTaskCommandTests : DatabaseTestsBase
    {
        private UpdateTaskCommandHandler _handler;
        private Guid _newConferenceId;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new UpdateTaskCommandHandler(context);
            _newConferenceId = Guid.Empty;
        }

        [TestCase(TaskType.Judge)]
        [TestCase(TaskType.Hearing)]
        [TestCase(TaskType.Participant)]
        public async Task Should_update_status_to_done(TaskType taskType)
        {
            const string body = "Automated Test Complete Task";
            const string updatedBy = "test@hmcts.net";
            _newConferenceId = Guid.NewGuid();
            var task = new Alert(_newConferenceId, _newConferenceId, body, taskType);
            await TestDataManager.SeedAlerts(new List<Alert>{task});
            
            var command = new UpdateTaskCommand(_newConferenceId, task.Id, updatedBy);
            await _handler.Handle(command);

            List<Alert> alerts;
            await using (var db = new VideoApiDbContext(VideoBookingsDbContextOptions))
            {
                alerts = await db.Tasks.Where(x => x.ConferenceId == command.ConferenceId).ToListAsync();
            }

            var updatedAlert = alerts.First(x => x.Id == task.Id);
            updatedAlert.Should().NotBeNull();
            updatedAlert.Status.Should().Be(TaskStatus.Done);
            updatedAlert.Updated.Should().NotBeNull();
            updatedAlert.UpdatedBy.Should().Be(updatedBy);
        }

        [Test]
        public void Should_throw_task_not_found_exception()
        {
            const string updatedBy = "test@hmcts.net";
            _newConferenceId = Guid.NewGuid();
            
            var command = new UpdateTaskCommand(_newConferenceId, 9999, updatedBy);
            Assert.ThrowsAsync<TaskNotFoundException>(() => _handler.Handle(command));
        }

        [TearDown]
        public async Task TearDown()
        {
            if (_newConferenceId != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test conference {_newConferenceId}");
                await TestDataManager.RemoveAlerts(_newConferenceId);
            }
        }
    }
}
