using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;

namespace VideoApi.DAL.Queries
{
    public class GetConferencesForTodayByUsernameQuery : IQuery
    {
        public string Username { get; set; }

        public GetConferencesForTodayByUsernameQuery(string username)
        {
            Username = username;
        }
    }

    public class GetConferencesForTodayByUsernameQueryHandler : IQueryHandler<GetConferencesForTodayByUsernameQuery, List<Conference>>
    {
        private readonly VideoApiDbContext _context;

        public GetConferencesForTodayByUsernameQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<Conference>> Handle(GetConferencesForTodayByUsernameQuery query)
        {
            query.Username = query.Username.ToLower().Trim();
            var today = DateTime.Today;
            var tomorrow = DateTime.Today.AddDays(1);

            var efQuery = _context.Conferences
                .Include("Participants.ParticipantStatuses")
                .Include("Participants.TestCallResult")
                .Include("ConferenceStatuses")
                .Include("Tasks").AsNoTracking()
                .Where(x => x.ScheduledDateTime >= today && x.ScheduledDateTime < tomorrow);
            
            return await efQuery
                .Where(x => x.Participants.Any(p => p.Username == query.Username))
                .OrderBy(x => x.ScheduledDateTime)
                .ToListAsync();
        }
    }
}