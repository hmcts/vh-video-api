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
            var conference = new ConferenceBuilder()
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
                var hearing = await db.Conferences.FindAsync(conferenceId);
                db.Remove(hearing);
                await db.SaveChangesAsync();
            }
        }
    }
}