using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;

namespace VideoApi.DAL.Queries
{
    public class GetConferencesForTodayByIndividualQuery : IQuery
    {
        public string Username { get; set; }

        public GetConferencesForTodayByIndividualQuery(string username)
        {
            Username = username;
        }
    }

    public class GetConferencesForTodayByIndividualQueryHandler : IQueryHandler<GetConferencesForTodayByIndividualQuery, List<Conference>>
    {
        private readonly VideoApiDbContext _context;

        public GetConferencesForTodayByIndividualQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<Conference>> Handle(GetConferencesForTodayByIndividualQuery query)
        {
            query.Username = query.Username.ToLower().Trim();
            var today = DateTime.Today;
            var tomorrow = DateTime.Today.AddDays(1);

            return await _context.Conferences
                .AsNoTracking()
                .Where(x => x.ScheduledDateTime >= today && x.ScheduledDateTime < tomorrow)
                .Where(x => x.Participants.Any(p => p.Username == query.Username))
                .OrderBy(x => x.ScheduledDateTime)
                .ToListAsync();
                
        }
    }
}
