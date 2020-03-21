using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
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
            const string body = "Automated Test Complete Task";
            const string updatedBy = "test@automated.com";

            var conference = new ConferenceBuilder(true)
                .WithParticipant(UserRole.Individual, "Claimant")
                .WithParticipant(UserRole.Judge, "Judge")
                .Build();

            var judge = conference.GetParticipants().First(x => x.UserRole == UserRole.Judge);
            var individual = conference.GetParticipants().First(x => x.UserRole == UserRole.Individual);

            var judgeTaskDone = new Task(judge.Id, body, TaskType.Judge);
            judgeTaskDone.CompleteTask(updatedBy);
            var participantTaskDone = new Task(individual.Id, body, TaskType.Participant);
            participantTaskDone.CompleteTask(updatedBy);
            var hearingTaskDone = new Task(conference.Id, body, TaskType.Hearing);
            hearingTaskDone.CompleteTask(updatedBy);

            conference.AddTask(judge.Id, TaskType.Judge, body);
            conference.AddTask(individual.Id, TaskType.Participant, body);
            conference.AddTask(conference.Id, TaskType.Hearing, body);
            conference.AddTask(individual.Id, TaskType.Participant, body);

            conference.GetTasks()[0].CompleteTask(updatedBy);

            var seededConference = await TestDataManager.SeedConference(conference);
            _newConferenceId = seededConference.Id;


            var query = new GetTasksForConferenceQuery(_newConferenceId);
            var results = await _handler.Handle(query);
            results.Count.Should().Be(conference.GetTasks().Count);
            results.Should().BeInDescendingOrder(x => x.Created);
        }

        [Test]
        public void Should_throw_conference_not_found_exception_when_conference_does_not_exist()
        {
            var conferenceId = Guid.NewGuid();
            var query = new GetTasksForConferenceQuery(conferenceId);
            Assert.ThrowsAsync<ConferenceNotFoundException>(() => _handler.Handle(query));
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