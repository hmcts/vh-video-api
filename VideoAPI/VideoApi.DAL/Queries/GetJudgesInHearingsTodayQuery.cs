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
    public class GetJudgesInHearingsTodayQuery : IQuery {}

    public class GetJudgesInHearingsTodayQueryHandler : IQueryHandler<GetJudgesInHearingsTodayQuery, List<Conference>>
    {
        private readonly VideoApiDbContext _context;

        public GetJudgesInHearingsTodayQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<Conference>> Handle(GetJudgesInHearingsTodayQuery query)
        {
            var today = DateTime.Today;
            var tomorrow = DateTime.Today.AddDays(1);

            return await _context.Conferences
                .Include(x => x.Participants)
                .AsNoTracking()
                .Where(x => x.ScheduledDateTime >= today && x.ScheduledDateTime < tomorrow)
                .Where(x => x.Participants.Any(p => p.UserRole == UserRole.Judge && p.State == ParticipantState.InHearing))
                .OrderBy(x => x.ScheduledDateTime)
                .ToListAsync();
        }
    }
}
