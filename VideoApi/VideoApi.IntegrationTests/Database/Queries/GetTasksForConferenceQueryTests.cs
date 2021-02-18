using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = VideoApi.Domain.Task;

namespace VideoApi.IntegrationTests.Database.Queries
{
    public class GetTasksForConferenceQueryTests : DatabaseTestsBase
    {
        private GetTasksForConferenceQueryHandler _handler;
        private Guid _newConferenceId;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new GetTasksForConferenceQueryHandler(context);
            _newConferenceId = Guid.Empty;
        }

        [Test]
        public async System.Threading.Tasks.Task Should_retrieve_all_alerts()
        {
            var conference = new ConferenceBuilder(true)
                .WithParticipant(UserRole.Individual, "Applicant")
                .WithParticipant(UserRole.Judge, "Judge")
                .Build();

            var seededConference = await TestDataManager.SeedConference(conference);
            _newConferenceId = seededConference.Id;

            var alerts = InitTasks(conference);
            await TestDataManager.SeedAlerts(alerts);

            var query = new GetTasksForConferenceQuery(_newConferenceId);
            var results = await _handler.Handle(query);
            results.Count.Should().Be(alerts.Count);
            results.Should().BeInDescendingOrder(x => x.Created);
        }

        private static List<Task> InitTasks(Conference conference)
        {
            const string body = "Automated Test Complete Task";
            const string updatedBy = "test@hmcts.net";

            var judge = conference.GetParticipants().First(x => x.UserRole == UserRole.Judge);
            var individual = conference.GetParticipants().First(x => x.UserRole == UserRole.Individual);

            var judgeTaskDone = new Task(conference.Id, judge.Id, body, TaskType.Judge);
            judgeTaskDone.CompleteTask(updatedBy);
            var participantTaskDone = new Task(conference.Id, individual.Id, body, TaskType.Participant);
            participantTaskDone.CompleteTask(updatedBy);
            var conferenceTaskDone = new Task(conference.Id, conference.Id, body, TaskType.Hearing);
            conferenceTaskDone.CompleteTask(updatedBy);

            var judgeTaskTodo = new Task(conference.Id, judge.Id, body, TaskType.Judge);
            var participantTaskTodo = new Task(conference.Id, individual.Id, body, TaskType.Participant);
            var conferenceTaskToDo = new Task(conference.Id, conference.Id, body, TaskType.Hearing);
            return new List<Task>
            {
                judgeTaskTodo,
                participantTaskTodo,
                conferenceTaskToDo,
                judgeTaskDone,
                participantTaskDone,
                conferenceTaskDone
            };
        }

        [TearDown]
        public async System.Threading.Tasks.Task TearDown()
        {
            if (_newConferenceId != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test conference {_newConferenceId}");
                await TestDataManager.RemoveConference(_newConferenceId);
            }
        }
    }
}
