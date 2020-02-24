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
    public class GetMonitoringMaxPercentageLostRecentQueryTests : DatabaseTestsBase
    {
        private GetMonitoringMaxPercentageLostRecentQueryHandler _handler;
        private Guid _newConferenceId;
        private Guid _newparticipantId;
        private VideoApiDbContext _context;

        [SetUp]
        public void Setup()
        {
            _context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new GetMonitoringMaxPercentageLostRecentQueryHandler(_context);
            _newConferenceId = Guid.Empty;
            _newparticipantId = Guid.Empty;
        }

        [Test]
        public async Task should_retrieve_all_monitoring_records()
        {
            _newConferenceId = Guid.NewGuid();
            _newparticipantId = Guid.NewGuid();
            
            var monitorings = new List<Monitoring>
            {
                new Monitoring(_newConferenceId, _newparticipantId, 0,0,0,0,0,0,0,0),
                new Monitoring(_newConferenceId, _newparticipantId, 0,0,0,0,0,0,0,0),
                new Monitoring(_newConferenceId, _newparticipantId, 0,0,0,0,0,0,0,0),
                new Monitoring(_newConferenceId, _newparticipantId, 0,0,0,0,0,0,0,0),
                new Monitoring(_newConferenceId, _newparticipantId, 0,0,0,0,0,0,0,0),
            };

            await AddMonitoringToDb(monitorings);

            var result = await _handler.Handle(new GetMonitoringMaxPercentageLostRecentQuery(_newConferenceId, _newparticipantId));

            result.Should().NotBeNull();
            result.Should().NotBeEmpty().And.HaveCount(monitorings.Count);
            result.Should().NotContainNulls();
            result.Should().BeInAscendingOrder(x => x.Timestamp);
            result.Should().OnlyContain(x => x.ConferenceId == _newConferenceId && x.ParticipantId == _newparticipantId);
        }

        private async Task AddMonitoringToDb(IEnumerable<Monitoring> monitorings)
        {
            await _context.Monitorings.AddRangeAsync(monitorings);
            await _context.SaveChangesAsync();
        }
        
        [TearDown]
        public async Task TearDown()
        {
            if (_newConferenceId != Guid.Empty && _newparticipantId != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test monitoring records {_newConferenceId} & {_newparticipantId}");
                await TestDataManager.RemoveMonitoring(_newConferenceId, _newparticipantId);
            }
        }
    }
}
