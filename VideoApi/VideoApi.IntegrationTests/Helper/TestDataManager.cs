using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Common.Security.Supplier.Kinly;
using VideoApi.Common.Security.Supplier.Vodafone;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Alert = VideoApi.Domain.Task;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Helper
{
    public class TestDataManager
    {
        private readonly KinlyConfiguration _kinlyConfiguration;
        private readonly VodafoneConfiguration _vodafoneConfiguration;
        private readonly DbContextOptions<VideoApiDbContext> _dbContextOptions;
        private readonly List<Guid> _seedeConferences = new();

        public TestDataManager(KinlyConfiguration kinlyConfiguration, 
            DbContextOptions<VideoApiDbContext> dbContextOptions,
            VodafoneConfiguration vodafoneConfiguration)
        {
            _kinlyConfiguration = kinlyConfiguration;
            _vodafoneConfiguration = vodafoneConfiguration;
            _dbContextOptions = dbContextOptions;
        }

        public async Task<Conference> SeedConference(Supplier supplier = Supplier.Vodafone)
        {
            var conference = new ConferenceBuilder(true)
                .WithParticipant(UserRole.Individual, "Applicant")
                .WithParticipant(UserRole.Representative, "Applicant")
                .WithParticipant(UserRole.Individual, "Respondent")
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithParticipant(UserRole.Judge, "Judge")
                .WithParticipant(UserRole.JudicialOfficeHolder, "PanelMember")
                .WithConferenceStatus(ConferenceState.InSession)
                .WithMeetingRoom(_kinlyConfiguration.PexipNode, _kinlyConfiguration.ConferenceUsername)
                .WithAudioRecordingRequired(false)
                .Build();
            var conferenceType = typeof(Conference);
            conferenceType.GetProperty("ActualStartTime")?.SetValue(conference, conference.ScheduledDateTime.AddMinutes(1));
            conference.SetProtectedProperty(nameof(conference.Supplier), supplier);

            foreach (var individual in conference.GetParticipants().Where(x => x.UserRole == UserRole.Individual))
            {
                individual.UpdateTestCallResult(true, TestScore.Okay);
            }
            
            return await SeedConference(conference);
        }
        
        public async Task<Conference> SeedConferenceWithEndpoint()
        {
            var conference = new ConferenceBuilder(true)
                .WithParticipant(UserRole.Individual, "Applicant", "TestApplicant@email.com")
                .WithParticipant(UserRole.Representative, "Applicant")
                .WithParticipant(UserRole.Individual, "Respondent")
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithParticipant(UserRole.Judge, "Judge")
                .WithParticipant(UserRole.JudicialOfficeHolder, "PanelMember")
                .WithEndpoint("Endpoint1", Guid.NewGuid().ToString(), "TestApplicant@email.com")
                .WithConferenceStatus(ConferenceState.InSession)
                .WithMeetingRoom(_kinlyConfiguration.PexipNode, _kinlyConfiguration.ConferenceUsername)
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

        public async Task<Conference> SeedConference(bool AudioRecording)
        {
            var conference = new ConferenceBuilder(true)
                .WithParticipant(UserRole.Individual, "Applicant")
                .WithParticipant(UserRole.Representative, "Applicant")
                .WithParticipant(UserRole.Individual, "Respondent")
                .WithParticipant(UserRole.Individual, "Interpreter")
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithParticipant(UserRole.Judge, "Judge")
                .WithParticipant(UserRole.JudicialOfficeHolder, "PanelMember")
                .WithConferenceStatus(ConferenceState.InSession)
                .WithMeetingRoom(_kinlyConfiguration.PexipNode, _kinlyConfiguration.ConferenceUsername)
                .WithAudioRecordingRequired(AudioRecording)
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
            _seedeConferences.Add(conference.Id);
            return conference;
        }

        public async Task<Conference> SeedConferenceWithLinkedParticipant()
        {
            var TestConference = new ConferenceBuilder()
               .WithParticipant(UserRole.Judge, null)
               .WithInterpreterLinkedParticipant(UserRole.Individual, "Applicant")
               .WithAudioRecordingRequired(true)
               .Build();

            return await SeedConference(TestConference);
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
            var command = new RemoveConferenceCommand(conferenceId);
            var handler = new RemoveConferenceCommandHandler(new VideoApiDbContext(_dbContextOptions));
            try
            {
                await handler.Handle(command);
            }
            catch (ConferenceNotFoundException)
            {
                TestContext.WriteLine($"Ignoring clean up for conference {conferenceId}. Does not exist");
            }
            
            
            
            await RemoveAlerts(conferenceId);
        }
        
        public async Task CleanUpSeededData()
        {
            foreach (var conferenceId in _seedeConferences)
            {
                TestContext.WriteLine($"Removing test conference {conferenceId}");
                await RemoveConference(conferenceId);
            }
            
            _seedeConferences.Clear();
        }
        
        public async Task RemoveConferences(List<Conference> conferences)
        {
            var conferenceIds = conferences.Select(conference => conference.Id).ToList();
            
            foreach (var id in conferenceIds)
            {
                await RemoveConference(id);
            }
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
            var roomsToDelete = db.Rooms.Include(x=> x.RoomParticipants).Where(x => x.ConferenceId == conferenceId);
            
            db.Rooms.RemoveRange(roomsToDelete);
            await db.SaveChangesAsync();
        }

        
        public async Task<List<Room>> SeedRooms(IEnumerable<Room> _rooms)
        {
            await using var db = new VideoApiDbContext(_dbContextOptions);
            List<Room> _seedRooms = _rooms.ToList();
            await db.Rooms.AddRangeAsync(_seedRooms);
            await db.SaveChangesAsync();

            return _seedRooms;
        }

        public async Task<ConsultationRoom> SeedRoom(ConsultationRoom consultationRoom)
        {
            await using var db = new VideoApiDbContext(_dbContextOptions);
            await db.Rooms.AddAsync(consultationRoom);
            await db.SaveChangesAsync();

            return consultationRoom;
        }

        public async Task<Room> SeedRoom(Room room)
        {
            await using var db = new VideoApiDbContext(_dbContextOptions);
            await db.Rooms.AddRangeAsync(room);
            await db.SaveChangesAsync();

            return room;
        }

        public async Task<List<Event>> SeedEvents(IEnumerable<Event> events)
        {
            await using var db = new VideoApiDbContext(_dbContextOptions);
            var seedEvents = events.ToList();
            await db.Events.AddRangeAsync(seedEvents);
            await db.SaveChangesAsync();

            return seedEvents;
        }

        public async Task RemoveEvents(Guid conferenceId, EventType eventType)
        {
            await using var db = new VideoApiDbContext(_dbContextOptions);
            var eventsToDelete = db.Events.Where(x => x.ConferenceId == conferenceId && x.EventType == eventType);

            db.Events.RemoveRange(eventsToDelete);
            await db.SaveChangesAsync();
        }

        public async Task RemoveEvents()
        {
            await using var db = new VideoApiDbContext(_dbContextOptions);
            var eventsToDelete = db.Events.Where(x => x.Reason.StartsWith("Automated"));
            db.Events.RemoveRange(eventsToDelete);
            await db.SaveChangesAsync();
        }

        public async Task SeedRoomWithRoomParticipant(long roomId, RoomParticipant roomParticipant)
        {
            await using var db = new VideoApiDbContext(_dbContextOptions);
            var room = db.Rooms.Include(x => x.RoomParticipants).First(x => x.Id == roomId);
            room.AddParticipant(roomParticipant);

            var participant = await db.Participants.FindAsync(roomParticipant.ParticipantId);
            participant.CurrentConsultationRoomId = roomId;
            
            await db.SaveChangesAsync();
        }

        public KinlyConfiguration GetKinlyConfiguration() => _kinlyConfiguration;

        public VodafoneConfiguration GetVodafoneConfiguration() => _vodafoneConfiguration;
    }
}
