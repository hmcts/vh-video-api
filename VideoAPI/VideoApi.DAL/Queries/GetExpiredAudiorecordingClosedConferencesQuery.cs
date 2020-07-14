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
    public class GetExpiredAudiorecordingClosedConferencesQuery : IQuery
    {
    }

    public class GetExpiredAudiorecordingClosedConferencesHandler : IQueryHandler<GetExpiredAudiorecordingClosedConferencesQuery, List<Conference>>
    {
        private readonly VideoApiDbContext _context;

        public GetExpiredAudiorecordingClosedConferencesHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<Conference>> Handle(GetExpiredAudiorecordingClosedConferencesQuery query)
        {
            return await _context.Conferences
                .Where(x => x.ScheduledDateTime.AddHours(14) <= DateTime.UtcNow
                && x.AudioRecordingRequired
                && x.State == ConferenceState.Closed)
                .OrderBy(x => x.ScheduledDateTime)
                .AsNoTracking()
                .ToListAsync();
        }
    }

}
