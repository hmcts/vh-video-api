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
    public class GetClosedConferencesQuery : IQuery
    {
    }

    public class GetClosedConferencesHandler : IQueryHandler<GetClosedConferencesQuery, List<Conference>>
    {
        private readonly VideoApiDbContext _context;
        public GetClosedConferencesHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<Conference>> Handle(GetClosedConferencesQuery query)
        {
            return await _context.Conferences
                .Include("Messages")
                .Where(x => x.Messages.Any())
                .Where(x => x.State == ConferenceState.Closed 
                    && x.ClosedDateTime.Value.AddMinutes(30) <= DateTime.UtcNow)
                    // && x.ClosedDateTime.HasValue 
                    // && x.ClosedDateTime.GetValueOrDefault().AddMinutes(30) <= DateTime.UtcNow)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
