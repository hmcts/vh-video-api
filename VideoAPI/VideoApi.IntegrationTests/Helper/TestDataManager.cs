using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Common.Configuration;
using VideoApi.DAL;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.IntegrationTests.Contexts;
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
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.VideoHearingsOfficer, null)
                .WithConferenceStatus(ConferenceState.InSession)
                .WithMeetingRoom(_services.PexipNode, _services.ConferenceUsername)
                .WithTask("Suspended", TaskType.Hearing)
                .Build();

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
        
        public async Task RemoveEvents()
        {
            using (var db = new VideoApiDbContext(_dbContextOptions))
            {
                var eventsToDelete = db.Events.Where(x => x.Reason.StartsWith("Automated"));
                db.Events.RemoveRange(eventsToDelete);
                await db.SaveChangesAsync();
            }
        }
    }
}