using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class CompleteAlertCommandTests : DatabaseTestsBase
    {
        private CompleteAlertCommandHandler _handler;
        private Guid _newConferenceId;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new CompleteAlertCommandHandler(context);
            _newConferenceId = Guid.Empty;
        }

        [TestCase(AlertType.Judge)]
        [TestCase(AlertType.Hearing)]
        [TestCase(AlertType.Participant)]
        public async Task should_update_status_to_done(AlertType alertType)
        {
            const string body = "Automated Test Complete Alert";
            const string updatedBy = "test@automated.com";
            var conferenceWithAlert = new ConferenceBuilder(true)
                .WithParticipant(UserRole.Individual, "Claimant")
                .WithAlert(body, alertType)
                .Build();
            var seededConference = await TestDataManager.SeedConference(conferenceWithAlert);
            _newConferenceId = seededConference.Id;
            var alert = seededConference.GetAlerts().First();

            var command = new CompleteAlertCommand(_newConferenceId, alert.Id, updatedBy);
            await _handler.Handle(command);
            
            Conference conference;
            using (var db = new VideoApiDbContext(VideoBookingsDbContextOptions))
            {
                conference = await db.Conferences.Include(x => x.Alerts)
                    .SingleAsync(x => x.Id == command.ConferenceId);
            }

            var updatedAlert = conference.GetAlerts().First(x => x.Id == alert.Id);
            updatedAlert.Should().NotBeNull();
            updatedAlert.Status.Should().Be(AlertStatus.Done);
            updatedAlert.Updated.Should().NotBeNull();
            updatedAlert.UpdatedBy.Should().Be(updatedBy);
        }
        
        
        [Test]
        public void should_throw_conference_not_found_exception()
        {
            const string updatedBy = "test@automated.com";
            var command = new CompleteAlertCommand(Guid.NewGuid(),9999, updatedBy);
            Assert.ThrowsAsync<ConferenceNotFoundException>(() => _handler.Handle(command));
        }
        
        [Test]
        public async Task should_throw_alert_not_found_exception()
        {
            const string body = "Automated Test Complete Alert";
            const string updatedBy = "test@automated.com";
            var conferenceWithAlert = new ConferenceBuilder(true)
                .WithParticipant(UserRole.Individual, "Claimant")
                .WithAlert(body, AlertType.Judge)
                .Build();
            var seededConference = await TestDataManager.SeedConference(conferenceWithAlert);
            _newConferenceId = seededConference.Id;

            var command = new CompleteAlertCommand(_newConferenceId, 9999, updatedBy);
            Assert.ThrowsAsync<AlertNotFoundException>(() => _handler.Handle(command));
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