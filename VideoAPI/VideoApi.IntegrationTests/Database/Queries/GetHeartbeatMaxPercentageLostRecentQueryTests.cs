using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using VideoApi.DAL;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Database.Queries
{
    public class GetHeartbeatMaxPercentageLostRecentQueryTests : DatabaseTestsBase
    {
        private GetHeartbeatMaxPercentageLostRecentQueryHandler _handler;
        private Guid _newConferenceId;
        private Guid _newParticipantId;
        private VideoApiDbContext _context;

        [SetUp]
        public void Setup()
        {
            _context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new GetHeartbeatMaxPercentageLostRecentQueryHandler(_context);
            _newConferenceId = Guid.Empty;
            _newParticipantId = Guid.Empty;
        }

        [Test]
        public async Task should_retrieve_all_heartbeat_records()
        {
            _newConferenceId = Guid.NewGuid();
            _newParticipantId = Guid.NewGuid();
            
            var heartbeats = new List<Heartbeat>
            {
                new Heartbeat(_newConferenceId, _newParticipantId, 1,1,1,1,1,1,1,1, "chrome", "1"),
                new Heartbeat(_newConferenceId, _newParticipantId, 1,1,1,1,1,1,1,1, "chrome", "1"),
                new Heartbeat(_newConferenceId, _newParticipantId, 1,1,1,1,1,1,1,1, "chrome", "1"),
                new Heartbeat(_newConferenceId, _newParticipantId, 1,1,1,1,1,1,1,1, "chrome", "1"),
                new Heartbeat(_newConferenceId, _newParticipantId, 1,1,1,1,1,1,1,1, "chrome", "1"),
            };

            await AddHeartbeatsToDb(heartbeats);

            var result = await _handler.Handle(new GetHeartbeatMaxPercentageLostRecentQuery(_newConferenceId, _newParticipantId));

            result.Should().NotBeNull();
            result.Should().NotBeEmpty().And.HaveCount(heartbeats.Count);
            result.Should().NotContainNulls();
            result.Should().BeInAscendingOrder(x => x.Timestamp);
            result.Should().OnlyContain
            (
                x => x.ConferenceId == _newConferenceId && 
                     x.ParticipantId == _newParticipantId &&
                     x.IncomingAudioPercentageLost == 1m &&
                     x.IncomingVideoPercentageLost == 1m &&
                     x.OutgoingAudioPercentageLost == 1m &&
                     x.OutgoingVideoPercentageLost == 1m &&
                     x.IncomingAudioPercentageLostRecent == 1m &&
                     x.IncomingVideoPercentageLostRecent == 1m &&
                     x.OutgoingAudioPercentageLostRecent == 1m &&
                     x.OutgoingVideoPercentageLostRecent == 1m &&
                     x.BrowserName == "chrome" &&
                     x.BrowserVersion == "1" 
            );
        }

        private async Task AddHeartbeatsToDb(IEnumerable<Heartbeat> heartbeats)
        {
            await _context.Heartbeats.AddRangeAsync(heartbeats);
            await _context.SaveChangesAsync();
        }
        
        [TearDown]
        public async Task TearDown()
        {
            if (_newConferenceId != Guid.Empty && _newParticipantId != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test Heartbeat records {_newConferenceId} & {_newParticipantId}");
                await TestDataManager.RemoveHeartbeats(_newConferenceId, _newParticipantId);
            }
        }
    }
}
