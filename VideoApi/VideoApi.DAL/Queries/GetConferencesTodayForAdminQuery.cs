using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.DAL.Queries
{
    public class GetConferencesTodayForAdminQuery : IQuery
    {
        public IEnumerable<string> UserNames { get; set; }
    }

    public class GetConferencesTodayForAdminQueryHandler : IQueryHandler<GetConferencesTodayForAdminQuery, List<Conference>>
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
                .Include(x => x.Participants).ThenInclude(x => x.LinkedParticipants)
                .AsNoTracking()
                .Where(x => x.ScheduledDateTime >= today && x.ScheduledDateTime < tomorrow);

            if (!query.UserNames.IsNullOrEmpty())
            {
                adminQuery = adminQuery.Where(p => p.Participants.Any(j => j.UserRole == UserRole.Judge
                    && query.UserNames.Contains(j.FirstName))).Distinct();
            }

            return await adminQuery
                .Where(x => x.MeetingRoom != null && x.MeetingRoom.AdminUri != null && x.MeetingRoom.JudgeUri != null
                            && x.MeetingRoom.ParticipantUri != null && x.MeetingRoom.PexipNode != null)
                .OrderBy(x => x.ScheduledDateTime)
                .ToListAsync();
        }
    }
}
