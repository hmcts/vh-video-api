using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;

namespace VideoApi.DAL.Queries
{
    public class GetConferencesTodayForAdminQuery : IQuery
    {
        public IEnumerable<string> VenueNames { get; set; }
    }

    public class
        GetConferencesTodayForAdminQueryHandler : IQueryHandler<GetConferencesTodayForAdminQuery, List<Conference>>
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

            var adminQuery = _context.Conferences
                .Include(x => x.Participants)
                .Include("Tasks")
                .AsNoTracking()
                .Where(x => x.ScheduledDateTime >= today && x.ScheduledDateTime < tomorrow);

            if (!query.VenueNames.IsNullOrEmpty())
            {
                adminQuery = adminQuery.Where(c => query.VenueNames.Contains(c.HearingVenueName));
            }

            return await adminQuery
                .OrderBy(x => x.ScheduledDateTime)
                .ToListAsync();
        }
    }
}
