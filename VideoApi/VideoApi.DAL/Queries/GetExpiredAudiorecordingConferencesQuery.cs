using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;

namespace VideoApi.DAL.Queries
{
    public class GetExpiredAudiorecordingConferencesQuery : IQuery
    {
    }

    public class GetExpiredAudiorecordingConferencesHandler : IQueryHandler<GetExpiredAudiorecordingConferencesQuery, List<Conference>>
    {
        private readonly VideoApiDbContext _context;

        public GetExpiredAudiorecordingConferencesHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<Conference>> Handle(GetExpiredAudiorecordingConferencesQuery query)
        {
            return await _context.Conferences
                .Where(x => x.ScheduledDateTime.AddHours(14) <= DateTime.UtcNow 
                && x.ScheduledDateTime >= DateTime.UtcNow.AddHours(-38)
                && x.State > 0)
                .OrderBy(x => x.ScheduledDateTime)
                .AsNoTracking()
                .ToListAsync();
        }
    }

}
