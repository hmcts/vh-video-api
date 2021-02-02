using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.DAL.Queries
{
    public class GetClosedConferencesWithInstantMessagesQuery : IQuery
    {
    }

    public class GetClosedConferencesWithInstantMessagesHandler : IQueryHandler<GetClosedConferencesWithInstantMessagesQuery, List<Conference>>
    {
        private readonly VideoApiDbContext _context;
        public GetClosedConferencesWithInstantMessagesHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<Conference>> Handle(GetClosedConferencesWithInstantMessagesQuery query)
        {
            return await _context.Conferences
                .Include(x=>x.InstantMessageHistory)
                .Where(x => x.InstantMessageHistory.Any())
                .Where(x => x.State == ConferenceState.Closed
                    && x.ClosedDateTime.HasValue
                    && x.ClosedDateTime.Value.AddMinutes(30) <= DateTime.UtcNow)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
