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
                .WithHearingTask("Suspended")
                .Build();

            foreach (var individual in conference.GetParticipants().Where(x => x.UserRole == UserRole.Individual))
            {
                individual.UpdateTestCallResult(true, TestScore.Okay);
            }
            
            return await SeedConference(conference);
        }

        public async Task<Conference> SeedConference(Conference conference)
        {
            using (var db = new VideoApiDbContext(_dbContextOptions))
            {
                await db.Conferences.AddAsync(conference);
                await db.SaveChangesAsync();
            }

            return conference;
        }

        public async Task RemoveConference(Guid conferenceId)
        {
            using (var db = new VideoApiDbContext(_dbContextOptions))
            {
                var conference = await db.Conferences
                    .Include("Participants.ParticipantStatuses")
                    .Include("ConferenceStatuses")
                    .SingleAsync(x => x.Id == conferenceId);

                db.Remove(conference);
                await db.SaveChangesAsync();
            }
        }
        
        public async Task RemoveConferences(List<Guid> conferenceIds)
        {
            await using (var db = new VideoApiDbContext(_dbContextOptions))
            {
                var conferences = await db.Conferences
                    .Include("Participants.ParticipantStatuses")
                    .Include("ConferenceStatuses")
                    .Where(x => conferenceIds.Contains(x.Id)).ToListAsync();

                db.RemoveRange(conferences);
                await db.SaveChangesAsync();
            }
        }

        public async Task RemoveEvents()
        {
            await using (var db = new VideoApiDbContext(_dbContextOptions))
            {
                var eventsToDelete = db.Events.Where(x => x.Reason.StartsWith("Automated"));
                db.Events.RemoveRange(eventsToDelete);
                await db.SaveChangesAsync();
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
        
        public async Task RemoveHeartbeats(Guid conferenceId, Guid participantId)
        {
            await using var db = new VideoApiDbContext(_dbContextOptions);
            var toDelete = db.Heartbeats.Where(x => x.ConferenceId == conferenceId && x.ParticipantId == participantId);
            db.Heartbeats.RemoveRange(toDelete);
            await db.SaveChangesAsync();
        }
    }
}
