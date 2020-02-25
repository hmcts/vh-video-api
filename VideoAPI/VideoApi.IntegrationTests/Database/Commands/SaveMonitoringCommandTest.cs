using System;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class SaveMonitoringCommandTest : DatabaseTestsBase
    {
        private SaveMonitoringCommandHandler _handler;
        private Guid _newConferenceId;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new SaveMonitoringCommandHandler(context);
            _newConferenceId = Guid.Empty;
        }

        [Test]
        public async Task should_save_monitoring()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            var participantId = seededConference.GetParticipants().First().Id;
            
            var command = new SaveMonitoringCommand(_newConferenceId, participantId, 1,1,1,1,1,1,1,1, "chrome", "1");
            await _handler.Handle(command);

            Monitoring savedMonitor;
            await using (var db = new VideoApiDbContext(VideoBookingsDbContextOptions))
            {
                savedMonitor = await db.Monitoring.FirstOrDefaultAsync(x =>
                    x.ConferenceId == _newConferenceId && x.ParticipantId == participantId);
            }

            savedMonitor.Should().NotBeNull();
            savedMonitor.ConferenceId.Should().Be(_newConferenceId);
            savedMonitor.ParticipantId.Should().Be(participantId);
            savedMonitor.OutgoingAudioPercentageLost.Should().Be(1);
            savedMonitor.OutgoingAudioPercentageLostRecent.Should().Be(1);
            savedMonitor.IncomingAudioPercentageLost.Should().Be(1);
            savedMonitor.IncomingAudioPercentageLostRecent.Should().Be(1);
            savedMonitor.OutgoingVideoPercentageLost.Should().Be(1);
            savedMonitor.OutgoingVideoPercentageLostRecent.Should().Be(1);
            savedMonitor.IncomingVideoPercentageLost.Should().Be(1);
            savedMonitor.IncomingVideoPercentageLostRecent.Should().Be(1);
            savedMonitor.BrowserName.Should().Be("chrome");
            savedMonitor.BrowserVersion.Should().Be("1");
            savedMonitor.Timestamp.Should().NotBe(new DateTime());
            savedMonitor.Timestamp.Should().BeAfter(DateTime.MinValue);
            savedMonitor.Timestamp.Should().BeBefore(DateTime.MaxValue);
        }

        [TearDown]
        public async Task TearDown()
        {
            if (_newConferenceId != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test conference {_newConferenceId}");
                await TestDataManager.RemoveConference(_newConferenceId);
                await TestDataManager.RemoveMonitoring(_newConferenceId);
            }

            await TestDataManager.RemoveEvents();
        }
    }
}
