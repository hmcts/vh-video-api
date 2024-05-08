using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.DAL.Queries;

public class GetActiveConferencesQuery : IQuery
{
}

public class GetActiveConferencesQueryHandler : IQueryHandler<GetActiveConferencesQuery, List<Conference>>
{
    private readonly VideoApiDbContext _context;

    public GetActiveConferencesQueryHandler(VideoApiDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<Conference>> Handle(GetActiveConferencesQuery query)
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);
        return await _context.Conferences
            .Include(c => c.Participants)
            .Where(c => c.State == ConferenceState.Paused
                        || c.State == ConferenceState.InSession
                        || (c.State == ConferenceState.Closed &&
                            c.Participants.Any(p => p.State == ParticipantState.InConsultation))
            )
            .Where(c => c.ScheduledDateTime >= today && c.ScheduledDateTime < tomorrow)
            .AsNoTracking()
            .ToListAsync();
    }
}
