using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Testing.Common.Helper.Builders;
using VideoApi.DAL;
using VideoApi.Domain;

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
                    .Include(x => x.Participants)
                    .Include(x => x.ConferenceStatuses)
                    .SingleAsync(x => x.Id == conferenceId);
                
                db.Remove(conference);
                await db.SaveChangesAsync();
            }
        }
    }
}