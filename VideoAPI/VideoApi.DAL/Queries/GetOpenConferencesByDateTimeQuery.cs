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
    public class GetOpenConferencesByDateTimeQuery : IQuery
    {
        public DateTime ScheduledDateTime { get; private set; }

        public GetOpenConferencesByDateTimeQuery(DateTime scheduledDateTime)
        {
            ScheduledDateTime = scheduledDateTime;
        }
    }

    public class GetOpenConferencesByDateTimeQueryHandler : IQueryHandler<GetOpenConferencesByDateTimeQuery, List<Conference>>
    {
        private readonly VideoApiDbContext _context;

        public GetOpenConferencesByDateTimeQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<Conference>> Handle(GetOpenConferencesByDateTimeQuery query)
        {
            return await _context.Conferences
                .Include("Participants.ParticipantStatuses")
                .Include("ConferenceStatuses")
                .Include("Tasks").AsNoTracking()
                .Where(x => x.ScheduledDateTime <= query.ScheduledDateTime)
                .Where(x => x.State < ConferenceState.Closed)
                .OrderBy(x => x.ScheduledDateTime)
                .ToListAsync();
        }
    }
}