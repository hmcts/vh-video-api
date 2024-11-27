using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;

namespace VideoApi.DAL.Queries;

public class GetConferencesTodayQuery : IQuery
{
    public IEnumerable<string> HearingVenueNames { get; set; }
}

public class GetConferencesTodayQueryHandler(VideoApiDbContext context)
    : IQueryHandler<GetConferencesTodayQuery, List<Conference>>
{
    public async Task<List<Conference>> Handle(GetConferencesTodayQuery query)
    {
        var today = DateTime.Today;
        var tomorrow = DateTime.Today.AddDays(1);
        
        var conferencesQueryable = context.Conferences
            .Include(x => x.Participants).ThenInclude(x => x.LinkedParticipants)
            .AsNoTracking()
            .Where(x => x.ScheduledDateTime >= today && x.ScheduledDateTime < tomorrow);
        
        if (query.HearingVenueNames is not null && query.HearingVenueNames.Any())
        {
            conferencesQueryable = conferencesQueryable.Where(p => query.HearingVenueNames.Contains(p.HearingVenueName)).Distinct();
        }
        
        return await conferencesQueryable
            .Where(x => x.MeetingRoom != null && x.MeetingRoom.AdminUri != null && x.MeetingRoom.JudgeUri != null
                        && x.MeetingRoom.ParticipantUri != null && x.MeetingRoom.PexipNode != null)
            .OrderBy(x => x.ScheduledDateTime)
            .ToListAsync();
    }
}
