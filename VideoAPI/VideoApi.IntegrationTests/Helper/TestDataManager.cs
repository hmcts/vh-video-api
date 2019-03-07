using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Testing.Common.Helper.Builders;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.IntegrationTests.Helper
{
    public class TestDataManager
    {
        private readonly DbContextOptions<VideoApiDbContext> _dbContextOptions;
        
        public TestDataManager(DbContextOptions<VideoApiDbContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }

        public async Task<Conference> SeedConference()
        {
            var conference = new ConferenceBuilder(true)
                .WithParticipant("Claimant LIP", "Claimant")
                .WithParticipant("Solicitor", "Claimant")
                .WithParticipant("Solicitor LIP", "Defendant")
                .WithParticipant("Solicitor", "Defendant")
                .WithConferenceStatus(ConferenceState.InSession)
                .Build();
            
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
                var eventsToDelete = db.Events.Where(x => x.ExternalEventId.StartsWith("Automated"));
                db.Events.RemoveRange(eventsToDelete);
                await db.SaveChangesAsync();
            }
        }
    }
}