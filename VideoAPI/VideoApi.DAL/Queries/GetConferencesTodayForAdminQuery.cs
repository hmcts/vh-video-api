using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;

namespace VideoApi.DAL.Queries
{
    public class GetConferencesTodayForAdminQuery : IQuery
    {
    }

    public class GetConferencesTodayForAdminQueryHandler : IQueryHandler<GetConferencesTodayForAdminQuery, List<Conference>>
    {
        private readonly VideoApiDbContext _context;

        public GetConferencesTodayForAdminQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<Conference>> Handle(GetConferencesTodayForAdminQuery query)
        {
            var today = DateTime.Today;
            var tomorrow = DateTime.Today.AddDays(1);
            return await _context.Conferences
                .Include(x => x.Participants)
                .Include("Tasks")
                .AsNoTracking()
                .Where(x => x.ScheduledDateTime >= today && x.ScheduledDateTime < tomorrow)
                .OrderBy(x => x.ScheduledDateTime)
                .Take(10)
                .ToListAsync();
        }
    }
}
