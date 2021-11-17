using Castle.Core.Internal;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;

namespace VideoApi.DAL.Queries
{
    public class GetConferencesTodayForStaffMemberByHearingVenueNameQuery : IQuery
    {
        public IEnumerable<string> HearingVenueNames { get; set; }
    }

    public class GetConferencesTodayForStaffMemberByHearingVenueNameQueryHandler : IQueryHandler<GetConferencesTodayForStaffMemberByHearingVenueNameQuery, List<Conference>>
    {
        private readonly VideoApiDbContext _context;

        public GetConferencesTodayForStaffMemberByHearingVenueNameQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<Conference>> Handle(GetConferencesTodayForStaffMemberByHearingVenueNameQuery query)
        {
            var today = DateTime.Today;
            var tomorrow = DateTime.Today.AddDays(1);

            return await _context.Conferences
                .Include(x => x.Participants)
                .Include(x => x.Endpoints)
                .Include(x => x.MeetingRoom)
                .AsNoTracking()
                .Where(x => x.ScheduledDateTime >= today && x.ScheduledDateTime < tomorrow)
                .Where(x => x.MeetingRoom != null && x.MeetingRoom.AdminUri != null && x.MeetingRoom.JudgeUri != null
                            && x.MeetingRoom.ParticipantUri != null && x.MeetingRoom.PexipNode != null)
                .Where(x => query.HearingVenueNames.Contains(x.HearingVenueName))
                .Distinct()
                .OrderBy(x => x.ScheduledDateTime)
                .ToListAsync();
        }
    }

}
