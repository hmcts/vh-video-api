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

            var command = new SaveHeartbeatCommand(new Heartbeat(_newConferenceId, participantId, 1, 1, 1, 1, 1, 1, 1, 1,
                DateTime.UtcNow, "chrome", "1", "Mac OS X", "10.15.7", 0, "25kbps", "opus", 1, 1, 0, 25, "2kbps", "H264", "640x480", "18kbps", "opus", 1, 0, "106kbps", "VP8", "1280x720", 1, 0));
            await _handler.Handle(command);

            Heartbeat savedHeartbeat;
            await using (var db = new VideoApiDbContext(VideoBookingsDbContextOptions))
            {
                savedHeartbeat = await db.Heartbeats.FirstOrDefaultAsync(x =>
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

            savedHeartbeat.OutgoingAudioPacketsLost.Should().Be(0);
            savedHeartbeat.OutgoingAudioBitrate.Should().Be("25kbps");
            savedHeartbeat.OutgoingAudioCodec.Should().Be("opus");
            savedHeartbeat.OutgoingAudioPacketSent.Should().Be(1);
            savedHeartbeat.OutgoingVideoPacketSent.Should().Be(1);
            savedHeartbeat.OutgoingVideoPacketsLost.Should().Be(0);
            savedHeartbeat.OutgoingVideoFramerate.Should().Be(25);
            savedHeartbeat.OutgoingVideoBitrate.Should().Be("2kbps");
            savedHeartbeat.OutgoingVideoCodec.Should().Be("H264");
            savedHeartbeat.OutgoingVideoResolution.Should().Be("640x480");
            savedHeartbeat.IncomingAudioBitrate.Should().Be("18kbps");
            savedHeartbeat.IncomingAudioCodec.Should().Be("opus");
            savedHeartbeat.IncomingAudioPacketReceived.Should().Be(1);
            savedHeartbeat.IncomingAudioPacketsLost.Should().Be(0);
            savedHeartbeat.IncomingVideoBitrate.Should().Be("106kbps");
            savedHeartbeat.IncomingVideoCodec.Should().Be("VP8");
            savedHeartbeat.IncomingVideoResolution.Should().Be("1280x720");
            savedHeartbeat.IncomingVideoPacketReceived.Should().Be(1);
            savedHeartbeat.IncomingVideoPacketsLost.Should().Be(0);
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
