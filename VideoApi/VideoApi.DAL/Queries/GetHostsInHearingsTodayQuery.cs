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
    public class GetHostsInHearingsTodayQuery : IQuery
    {
        public bool JudgesOnly { get; set; }

        public GetHostsInHearingsTodayQuery(bool judgesOnly = false)
        {
            JudgesOnly = judgesOnly;
        }
    }

    public class GetHostsInHearingsTodayQueryHandler : IQueryHandler<GetHostsInHearingsTodayQuery, List<Conference>>
    {
        private readonly VideoApiDbContext _context;

        public GetHostsInHearingsTodayQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<Conference>> Handle(GetHostsInHearingsTodayQuery query)
        {
            var today = DateTime.Today;
            var tomorrow = DateTime.Today.AddDays(1);

            var queryToExecute = _context.Conferences
                .Include(x => x.Participants)
                .AsNoTracking()
                .Where(x => x.ScheduledDateTime >= today && x.ScheduledDateTime < tomorrow)
                .Where(x => x.Participants.Any(p =>
                    (p.State == ParticipantState.InHearing || p.State == ParticipantState.Available)));

            if (query.JudgesOnly)
                queryToExecute = queryToExecute.Where(x => x.Participants.Any(p => p.UserRole == UserRole.Judge));
            else
                queryToExecute = queryToExecute
                    .Where(x => x.Participants.Any(p => p.UserRole == UserRole.Judge || p.UserRole == UserRole.StaffMember));

            return await queryToExecute.OrderBy(x => x.ScheduledDateTime).ToListAsync();
        }
    }
}
