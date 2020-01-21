using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.DAL.Queries
{
    public class GetExpiredUnclosedConferencesQuery : IQuery
    {
        public GetExpiredUnclosedConferencesQuery()
        {
        }
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
                .Include("Participants.ParticipantStatuses")
                .Include("ConferenceStatuses")
                .Include("Tasks").AsNoTracking()
                .Where(x => x.ScheduledDateTime.AddHours(14) <= DateTime.UtcNow
                            && x.State < ConferenceState.Closed)
                // .Where(x => x.ScheduledDateTime <= query.ScheduledDateTime
                //     && x.State < ConferenceState.Closed)
                .OrderBy(x => x.ScheduledDateTime)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}