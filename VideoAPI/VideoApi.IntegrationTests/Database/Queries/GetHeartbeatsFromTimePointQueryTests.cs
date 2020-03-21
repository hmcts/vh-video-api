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
    public class GetHeartbeatsFromTimePointQueryTests : DatabaseTestsBase
    {
        private GetHeartbeatsFromTimePointQueryHandler _handler;
        private Guid _newConferenceId;
        private Guid _newParticipantId;
        private VideoApiDbContext _context;

        [SetUp]
        public void Setup()
        {
            _context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new GetHeartbeatsFromTimePointQueryHandler(_context);
            _newConferenceId = Guid.Empty;
            _newParticipantId = Guid.Empty;
        }

        [Test]
        public async Task Should_retrieve_all_heartbeat_records()
        {
            _newConferenceId = Guid.NewGuid();
            _newParticipantId = Guid.NewGuid();
            
            var heartbeats = new List<Heartbeat>
            {
                new Heartbeat(_newConferenceId, _newParticipantId, 1,1,1,1,1,1,1,1, DateTime.UtcNow, "chrome", "1"),
                new Heartbeat(_newConferenceId, _newParticipantId, 1,1,1,1,1,1,1,1, DateTime.UtcNow, "chrome", "1"),
                new Heartbeat(_newConferenceId, _newParticipantId, 1,1,1,1,1,1,1,1, DateTime.UtcNow, "chrome", "1"),
                new Heartbeat(_newConferenceId, _newParticipantId, 1,1,1,1,1,1,1,1, DateTime.UtcNow, "chrome", "1"),
                new Heartbeat(_newConferenceId, _newParticipantId, 1,1,1,1,1,1,1,1, DateTime.UtcNow, "chrome", "1"),
            };

            await AddHeartbeatsToDb(heartbeats);

            var result = await _handler.Handle(new GetHeartbeatsFromTimePointQuery
            (
                _newConferenceId, _newParticipantId, TimeSpan.FromMinutes(15)
            ));

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
                     x.Timestamp != DateTime.MinValue && 
                     x.Timestamp != DateTime.MaxValue &&
                     x.BrowserName == "chrome" &&
                     x.BrowserVersion == "1" 
            );
        }
        
                [Test]
        public async Task Should_retrieve_all_heartbeat_records_within_timespan()
        {
            _newConferenceId = Guid.NewGuid();
            _newParticipantId = Guid.NewGuid();
            
            var heartbeats = new List<Heartbeat>
            {
                new Heartbeat(_newConferenceId, _newParticipantId, 1,1,1,1,1,1,1,1, DateTime.UtcNow.AddMinutes(-30), "chrome", "1"),
                new Heartbeat(_newConferenceId, _newParticipantId, 1,1,1,1,1,1,1,1, DateTime.UtcNow, "chrome", "1"),
                new Heartbeat(_newConferenceId, _newParticipantId, 1,1,1,1,1,1,1,1, DateTime.UtcNow, "chrome", "1"),
                new Heartbeat(_newConferenceId, _newParticipantId, 1,1,1,1,1,1,1,1, DateTime.UtcNow, "chrome", "1"),
                new Heartbeat(_newConferenceId, _newParticipantId, 1,1,1,1,1,1,1,1, DateTime.UtcNow, "chrome", "1"),
            };

            await AddHeartbeatsToDb(heartbeats);

            var result = await _handler.Handle(new GetHeartbeatsFromTimePointQuery
            (
                _newConferenceId, _newParticipantId, TimeSpan.FromMinutes(15)
            ));

            result.Should().NotBeNull();
            result.Should().NotBeEmpty().And.HaveCount(4);
            result.Should().NotContainNulls();
            result.Should().BeInAscendingOrder(x => x.Timestamp);
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
