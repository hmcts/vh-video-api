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
    public class SaveHeartbeatCommandTest : DatabaseTestsBase
    {
        private SaveHeartbeatCommandHandler _handler;
        private Guid _newConferenceId;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new SaveHeartbeatCommandHandler(context);
            _newConferenceId = Guid.Empty;
        }

        [Test]
        public async Task Should_save_heartbeats()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            var participantId = seededConference.GetParticipants().First().Id;

            var command = new SaveHeartbeatCommand(_newConferenceId, participantId, 1, 1, 1, 1, 1, 1, 1, 1,
                DateTime.UtcNow, "chrome", "1", "Mac OS X", "10.15.7");
            await _handler.Handle(command);

            Heartbeat savedHeartbeat;
            await using (var db = new VideoApiDbContext(VideoBookingsDbContextOptions))
            {
                savedHeartbeat = await db.Heartbeats.AsQueryable().FirstOrDefaultAsync(x =>
                    x.ConferenceId == _newConferenceId && x.ParticipantId == participantId);
            }

            savedHeartbeat.Should().NotBeNull();
            savedHeartbeat.ConferenceId.Should().Be(_newConferenceId);
            savedHeartbeat.ParticipantId.Should().Be(participantId);
            savedHeartbeat.OutgoingAudioPercentageLost.Should().Be(1);
            savedHeartbeat.OutgoingAudioPercentageLostRecent.Should().Be(1);
            savedHeartbeat.IncomingAudioPercentageLost.Should().Be(1);
            savedHeartbeat.IncomingAudioPercentageLostRecent.Should().Be(1);
            savedHeartbeat.OutgoingVideoPercentageLost.Should().Be(1);
            savedHeartbeat.OutgoingVideoPercentageLostRecent.Should().Be(1);
            savedHeartbeat.IncomingVideoPercentageLost.Should().Be(1);
            savedHeartbeat.IncomingVideoPercentageLostRecent.Should().Be(1);
            savedHeartbeat.BrowserName.Should().Be("chrome");
            savedHeartbeat.BrowserVersion.Should().Be("1");
            savedHeartbeat.Timestamp.Should().NotBe(new DateTime());
            savedHeartbeat.Timestamp.Should().BeAfter(DateTime.MinValue);
            savedHeartbeat.Timestamp.Should().BeBefore(DateTime.MaxValue);
        }

        [TearDown]
        public async Task TearDown()
        {
            if (_newConferenceId != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test conference {_newConferenceId}");
                await TestDataManager.RemoveConference(_newConferenceId);
                await TestDataManager.RemoveHeartbeats(_newConferenceId);
            }

            await TestDataManager.RemoveEvents();
        }
    }
}
