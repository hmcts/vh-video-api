using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = VideoApi.Domain.Task;
using TaskStatus = VideoApi.Domain.Enums.TaskStatus;

namespace VideoApi.IntegrationTests.Database.Queries
{
    public class GetIncompleteTasksForConferenceQueryTests : DatabaseTestsBase
    {
        private GetIncompleteTasksForConferenceQueryHandler _handler;
        private Guid _newConferenceId;
        
        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new GetIncompleteTasksForConferenceQueryHandler(context);
            _newConferenceId = Guid.Empty;
        }

        [Test]
        public async System.Threading.Tasks.Task should_retrieve_alerts_with_todo_status()
        {
            const string body = "Automated Test Complete Task";
            const string updatedBy = "test@automated.com";
            
            var judgeAlertDone = new Task(body, TaskType.Judge);
            judgeAlertDone.CompleteTask(updatedBy);
            var participantAlertDone = new Task(body, TaskType.Participant);
            participantAlertDone.CompleteTask(updatedBy);
            var hearingAlertDone = new Task(body, TaskType.Hearing);
            hearingAlertDone.CompleteTask(updatedBy);
            
            var conference = new ConferenceBuilder(true)
                .WithParticipant(UserRole.Individual, "Claimant")
                .Build();
            
            conference.AddTask(TaskType.Judge, body);
            conference.AddTask(TaskType.Participant, body);
            conference.AddTask(TaskType.Hearing, body);
            conference.AddTask(TaskType.Participant, body);

            conference.GetTasks()[0].CompleteTask(updatedBy);
            
            var seededConference = await TestDataManager.SeedConference(conference);
            _newConferenceId = seededConference.Id;
           

            var query = new GetIncompleteTasksForConferenceQuery(_newConferenceId);
            var results = await _handler.Handle(query);
            results.Any(x => x.Status == TaskStatus.Done).Should().BeFalse();
        }
        
        [Test]
        public void should_throw_conference_not_found_exception_when_conference_does_not_exist()
        {
            var conferenceId = Guid.NewGuid();
            var query = new GetIncompleteTasksForConferenceQuery(conferenceId);
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