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

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class AddAlertCommandTests : DatabaseTestsBase
    {
        private AddAlertCommandHandler _handler;
        private Guid _newConferenceId;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new AddAlertCommandHandler(context);
            _newConferenceId = Guid.Empty;
        }

        [TestCase(AlertType.Judge)]
        [TestCase(AlertType.Hearing)]
        [TestCase(AlertType.Participant)]
        public async Task should_add_an_alert(AlertType alertType)
        {
            var seededConference = await TestDataManager.SeedConference();
            _newConferenceId = seededConference.Id;
            const string body = "Automated Test Add Alert";

            var command = new AddAlertCommand(_newConferenceId, body, alertType);
            await _handler.Handle(command);

            Conference conference;
            using (var db = new VideoApiDbContext(VideoBookingsDbContextOptions))
            {
                conference = await db.Conferences.Include(x => x.Alerts)
                    .SingleAsync(x => x.Id == command.ConferenceId);
            }

            var savedAlert = conference.GetAlerts().First(x => x.Body == body && x.Type == alertType);

            savedAlert.Should().NotBeNull();
            savedAlert.Status.Should().Be(AlertStatus.ToDo);
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