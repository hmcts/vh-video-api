using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.DAL.Queries
{
    [SuppressMessage("Minor Code Smell", "S2094:Classes should not be empty")]
    public class GetExpiredUnclosedConferencesQuery : IQuery
    {
    }

    public class GetExpiredUnclosedConferencesHandler : IQueryHandler<GetExpiredUnclosedConferencesQuery, List<Conference>>
    {
        private readonly VideoApiDbContext _context;

        public GetExpiredUnclosedConferencesHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<Conference>> Handle(GetExpiredUnclosedConferencesQuery query)
        {
            return await _context.Conferences
                .Where(x => x.ScheduledDateTime.AddHours(14) <= DateTime.UtcNow
                            && x.State < ConferenceState.Closed)
                .OrderBy(x => x.ScheduledDateTime)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
