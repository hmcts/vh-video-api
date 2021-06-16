using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class RemoveHeartbeatsForConferencesCommandTests : DatabaseTestsBase
    {
        private SaveHeartbeatCommandHandler _saveHeartbeatHandler;
        private RemoveHeartbeatsForConferencesCommandHandler _handler;
        private Guid _conference1Id;
        private List<Conference> _conferenceList;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new RemoveHeartbeatsForConferencesCommandHandler(context);
            _saveHeartbeatHandler = new SaveHeartbeatCommandHandler(context);
            _conference1Id = Guid.Empty;
        }

        [Test]
        public async Task Should_remove_heartbeats_for_conferences_older_than_14_days()
        {
            _conferenceList = new List<Conference>();
            var utcDate = DateTime.UtcNow;
            var hearingOlderThan14Days = utcDate.AddDays(-14);

            var conference1 = new ConferenceBuilder(true, scheduledDateTime: hearingOlderThan14Days)
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Closed)
                .Build();
            _conference1Id = conference1.Id;
            var conference1ParticipantId = conference1.GetParticipants().First().Id;
            _conferenceList.Add(conference1);

            foreach (var c in _conferenceList)
            {
                await TestDataManager.SeedConference(c);
            }

            var command = new SaveHeartbeatCommand(_conference1Id, conference1ParticipantId, 0.00M, 0.00M, 0.40M, 0.10M,
                0.00M, 0.00M, 0.50M, 0.20M, DateTime.UtcNow, "Chrome", "84.0.4147.105", "Mac OS X", "10.15.7");
            await _saveHeartbeatHandler.Handle(command);
            command = new SaveHeartbeatCommand(_conference1Id, conference1ParticipantId, 0.00M, 0.00M, 0.50M, 1.50M,
                0.00M, 0.00M, 0.50M, 1.50M, DateTime.UtcNow, "Chrome", "84.0.4147.105", "Mac OS X", "10.15.7");
            await _saveHeartbeatHandler.Handle(command);

            var removeCommand = new RemoveHeartbeatsForConferencesCommand();
            await _handler.Handle(removeCommand);

            List<Heartbeat> heartbeats;
            await using (var db = new VideoApiDbContext(VideoBookingsDbContextOptions))
            {
                heartbeats = await db.Heartbeats.AsQueryable().Where(x => x.ConferenceId == _conference1Id).ToListAsync();
            }

            var afterCount = heartbeats.Count;
            afterCount.Should().Be(0);
        }

        [Test]
        public async Task Should_not_remove_heartbeats_for_conferences_within_14_days()
        {
            _conferenceList = new List<Conference>();
            var utcDate = DateTime.UtcNow;
            var hearingWithin14Days = utcDate.AddDays(-13);

            var conference1 = new ConferenceBuilder(true, scheduledDateTime: hearingWithin14Days)
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Closed)
                .Build();
            _conference1Id = conference1.Id;
            var participantId = conference1.GetParticipants().First().Id;
            _conferenceList.Add(conference1);
            foreach (var c in _conferenceList)
            {
                await TestDataManager.SeedConference(c);
            }

            var command = new SaveHeartbeatCommand(_conference1Id, participantId, 0.00M, 0.00M, 0.40M, 0.10M, 0.00M,
                0.00M, 0.50M, 0.20M, DateTime.UtcNow, "Chrome", "84.0.4147.105", "Mac OS X", "10.15.7");
            await _saveHeartbeatHandler.Handle(command);
            command = new SaveHeartbeatCommand(_conference1Id, participantId, 0.00M, 0.00M, 0.50M, 1.50M, 0.00M, 0.00M,
                0.50M, 1.50M, DateTime.UtcNow, "Chrome", "84.0.4147.105", "Mac OS X", "10.15.7");
            await _saveHeartbeatHandler.Handle(command);
            command = new SaveHeartbeatCommand(_conference1Id, participantId, 0.30M, 0.15M, 0.60M, 1.50M, 0.00M, 0.00M,
                0.80M, 1.50M, DateTime.UtcNow, "Chrome", "84.0.4147.105", "Mac OS X", "10.15.7");
            await _saveHeartbeatHandler.Handle(command);

            var removeCommand = new RemoveHeartbeatsForConferencesCommand();
            await _handler.Handle(removeCommand);

            List<Heartbeat> heartbeats;
            await using (var db = new VideoApiDbContext(VideoBookingsDbContextOptions))
            {
                heartbeats = await db.Heartbeats.AsQueryable().Where(x => x.ConferenceId == _conference1Id).ToListAsync();
            }

            var afterCount = heartbeats.Count;
            afterCount.Should().Be(3);
        }

        [TearDown]
        public async Task TearDown()
        {
            if (_conference1Id != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test conference {_conference1Id}");
                await TestDataManager.RemoveConference(_conference1Id);
                await TestDataManager.RemoveHeartbeats(_conference1Id);
            }
        }
    }
}
