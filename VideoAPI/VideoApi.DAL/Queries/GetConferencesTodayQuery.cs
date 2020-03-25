using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;

namespace VideoApi.DAL.Queries
{
    public class GetConferencesTodayQuery : IQuery
    {
    }

    public class GetConferencesTodayQueryHandler : IQueryHandler<GetConferencesTodayQuery, List<Conference>>
    {
        private readonly VideoApiDbContext _context;

        public GetConferencesTodayQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<Conference>> Handle(GetConferencesTodayQuery query)
        {
            var today = DateTime.Today;
            var tomorrow = DateTime.Today.AddDays(1);
            return await _context.Conferences
                .Include(x => x.Participants)
                .Include("Tasks")
                .AsNoTracking()
                .Where(x => x.ScheduledDateTime >= today && x.ScheduledDateTime < tomorrow)
                .OrderBy(x => x.ScheduledDateTime)
                .ToListAsync();
        }
    }
}
