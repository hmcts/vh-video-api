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

namespace VideoApi.IntegrationTests.Database.Queries
{
    public class GetIncompleteAlertsForConferenceQueryTests : DatabaseTestsBase
    {
        private GetIncompleteAlertsForConferenceQueryHandler _handler;
        private Guid _newConferenceId;
        
        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new GetIncompleteAlertsForConferenceQueryHandler(context);
            _newConferenceId = Guid.Empty;
        }

        [Test]
        public async Task should_retrieve_alerts_with_todo_status()
        {
            const string body = "Automated Test Complete Alert";
            const string updatedBy = "test@automated.com";
            
            var judgeAlertDone = new Alert(body, AlertType.Judge);
            judgeAlertDone.CompleteTask(updatedBy);
            var participantAlertDone = new Alert(body, AlertType.Participant);
            participantAlertDone.CompleteTask(updatedBy);
            var hearingAlertDone = new Alert(body, AlertType.Hearing);
            hearingAlertDone.CompleteTask(updatedBy);
            
            var conference = new ConferenceBuilder(true)
                .WithParticipant(UserRole.Individual, "Claimant")
                .Build();
            
            conference.AddAlert(AlertType.Judge, body);
            conference.AddAlert(AlertType.Participant, body);
            conference.AddAlert(AlertType.Hearing, body);
            conference.AddAlert(AlertType.Participant, body);

            conference.GetAlerts()[0].CompleteTask(updatedBy);
            
            var seededConference = await TestDataManager.SeedConference(conference);
            _newConferenceId = seededConference.Id;
           

            var query = new GetIncompleteAlertsForConferenceQuery(_newConferenceId);
            var results = await _handler.Handle(query);
            results.Any(x => x.Status == AlertStatus.Done).Should().BeFalse();
        }
        
        [Test]
        public void should_throw_conference_not_found_exception_when_conference_does_not_exist()
        {
            var conferenceId = Guid.NewGuid();
            var query = new GetIncompleteAlertsForConferenceQuery(conferenceId);
            Assert.ThrowsAsync<ConferenceNotFoundException>(() => _handler.Handle(query));
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