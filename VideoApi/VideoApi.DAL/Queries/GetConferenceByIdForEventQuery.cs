using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;

namespace VideoApi.DAL.Queries;

public class GetConferenceByIdForEventQuery(Guid conferenceId) : IQuery
{
    public Guid ConferenceId { get; set; } = conferenceId;
}

public class GetConferenceByIdForEventQueryHandler(VideoApiDbContext context)
    : IQueryHandler<GetConferenceByIdForEventQuery, Conference>
{
    public async Task<Conference> Handle(GetConferenceByIdForEventQuery query)
    {
        return await context.Conferences
            .Include(x => x.Participants).ThenInclude(x => x.CurrentConsultationRoom)
            .Include(x => x.Participants).ThenInclude(x => x.LinkedParticipants).ThenInclude(x => x.Linked)
            .Include(x => x.Endpoints).ThenInclude(e => e.ParticipantsLinked)
            .Include(x => x.Rooms).ThenInclude(x => x.RoomParticipants)
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == query.ConferenceId);
    }
}
