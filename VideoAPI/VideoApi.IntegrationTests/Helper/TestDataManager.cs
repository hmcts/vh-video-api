using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Common.Configuration;
using VideoApi.DAL;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Alert = VideoApi.Domain.Task;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Helper
{
    public class TestDataManager
    {
        private readonly ServicesConfiguration _services;
        private readonly DbContextOptions<VideoApiDbContext> _dbContextOptions;

        public TestDataManager(ServicesConfiguration services, DbContextOptions<VideoApiDbContext> dbContextOptions)
        {
            _services = services;
            _dbContextOptions = dbContextOptions;
        }

        public async Task<Conference> SeedConference()
        {
            var conference = new ConferenceBuilder(true)
                .WithParticipant(UserRole.Individual, "Claimant")
                .WithParticipant(UserRole.Representative, "Claimant")
                .WithParticipant(UserRole.Individual, "Defendant")
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.InSession)
                .WithMeetingRoom(_services.PexipNode, _services.ConferenceUsername)
                .WithAudioRecordingRequired(false)
                .Build();
            var conferenceType = typeof(Conference);
            conferenceType.GetProperty("ActualStartTime")?.SetValue(conference, conference.ScheduledDateTime.AddMinutes(1));

            foreach (var individual in conference.GetParticipants().Where(x => x.UserRole == UserRole.Individual))
            {
                individual.UpdateTestCallResult(true, TestScore.Okay);
            }
            
            return await SeedConference(conference);
        }

        public async Task<Conference> SeedConference(Conference conference)
        {
            await using var db = new VideoApiDbContext(_dbContextOptions);
            await db.Conferences.AddAsync(conference);
            await db.SaveChangesAsync();

            return conference;
        }

        public async Task<List<Alert>> SeedAlerts(IEnumerable<Alert> alerts)
        {
            await using var db = new VideoApiDbContext(_dbContextOptions);
            var seedAlerts = alerts.ToList();
            await db.Tasks.AddRangeAsync(seedAlerts);
            await db.SaveChangesAsync();

            return seedAlerts;
        }

        public async Task RemoveConference(Guid conferenceId)
        {
            await using var db = new VideoApiDbContext(_dbContextOptions);
            var conference = await db.Conferences
                .Include("Endpoints")
                .Include("Participants.ParticipantStatuses")
                .Include("ConferenceStatuses")
                .SingleAsync(x => x.Id == conferenceId);

            db.Remove(conference);
            await db.SaveChangesAsync();

            await RemoveAlerts(conferenceId);
        }
        
        public async Task RemoveConferences(List<Conference> conferences)
        {
            await using var db = new VideoApiDbContext(_dbContextOptions);
            var conferenceIds = conferences.Select(conference => conference.Id).ToList();
            var allConferences = await db.Conferences
                .Include("Endpoints")
                .Include("Participants.ParticipantStatuses")
                .Include("ConferenceStatuses")
                .Where(x => conferenceIds.Contains(x.Id)).ToListAsync();

            db.RemoveRange(allConferences);
            await db.SaveChangesAsync();

            foreach (var id in conferenceIds)
            {
                await RemoveAlerts(id);
            }
        }

        public async Task RemoveEvents()
        {
            await using var db = new VideoApiDbContext(_dbContextOptions);
            var eventsToDelete = db.Events.Where(x => x.Reason.StartsWith("Automated"));
            db.Events.RemoveRange(eventsToDelete);
            await db.SaveChangesAsync();
        }
        
        public async Task SeedHeartbeats(IEnumerable<Heartbeat> heartbeats)
        {
            await using var db = new VideoApiDbContext(_dbContextOptions);
            await db.Heartbeats.AddRangeAsync(heartbeats);
            await db.SaveChangesAsync();
        }
        
        public async Task RemoveHeartbeats(Guid conferenceId)
        {
            await using var db = new VideoApiDbContext(_dbContextOptions);
            var toDelete = db.Heartbeats.Where(x => x.ConferenceId == conferenceId);
            db.Heartbeats.RemoveRange(toDelete);
            await db.SaveChangesAsync();
        }
        
        public async Task RemoveHeartbeats(List<Conference> conferences)
        {
            var conferenceIds = conferences.Select(conference => conference.Id).ToList();
            await using var db = new VideoApiDbContext(_dbContextOptions);
            var heartbeats = await db.Heartbeats
                .Where(x => conferenceIds.Contains(x.ConferenceId))
                .ToListAsync();

            db.RemoveRange(heartbeats);
            await db.SaveChangesAsync();
        }
        
        public async Task RemoveHeartbeats(Guid conferenceId, Guid participantId)
        {
            await using var db = new VideoApiDbContext(_dbContextOptions);
            var toDelete = db.Heartbeats.Where(x => x.ConferenceId == conferenceId && x.ParticipantId == participantId);
            db.Heartbeats.RemoveRange(toDelete);
            await db.SaveChangesAsync();
        }

        public async Task RemoveAlerts(Guid conferenceId)
        {
            await using var db = new VideoApiDbContext(_dbContextOptions);
            var toDelete = db.Tasks.Where(x => x.ConferenceId == conferenceId);
            db.Tasks.RemoveRange(toDelete);
            await db.SaveChangesAsync();
        }

        public async Task RemoveRooms(Guid conferenceId)
        {
            await using var db = new VideoApiDbContext(_dbContextOptions);
            var roomsToDelete = db.Rooms.Where(x => x.ConferenceId == conferenceId);
            db.Rooms.RemoveRange(roomsToDelete);
            await db.SaveChangesAsync();
        }

        public async Task<Room> SeedRoom(Room room)
        {
            await using var db = new VideoApiDbContext(_dbContextOptions);
            await db.Rooms.AddAsync(room);
            await db.SaveChangesAsync();

            return room;
        }

        public async Task<Room> SeedRoomWithRoomParticipant(long roomId, RoomParticipant roomParticipant)
        {
            await using var db = new VideoApiDbContext(_dbContextOptions);
            var room = db.Rooms.Where(x => x.Id == roomId).FirstOrDefault();
            room.AddParticipant(roomParticipant);
            await db.SaveChangesAsync();
            return room;
        }

        public async Task<Room> GetRoomById(long roomId)
        {
            await using var db = new VideoApiDbContext(_dbContextOptions);
            var room = db.Rooms.Include("RoomParticipants").Where(x => x.Id == roomId).FirstOrDefault();

            return room;
        }
    }
}
