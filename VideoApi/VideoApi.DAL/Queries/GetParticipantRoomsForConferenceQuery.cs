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
    public class GetParticipantRoomsForConferenceQuery : IQuery
    {
        public GetParticipantRoomsForConferenceQuery(Guid conferenceId)
        {
            ConferenceId = conferenceId;
        }

        public Guid ConferenceId { get; }
    }
    
    public class GetParticipantRoomsForConferenceQueryHandler : IQueryHandler<GetParticipantRoomsForConferenceQuery, List<ParticipantRoom>>
    {
        private readonly VideoApiDbContext _context;

        public GetParticipantRoomsForConferenceQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public Task<List<ParticipantRoom>> Handle(GetParticipantRoomsForConferenceQuery query)
        {
            return _context.Rooms.OfType<ParticipantRoom>()
                .Include(x => x.RoomParticipants)
                .AsNoTracking()
                .Where(x => x.ConferenceId == query.ConferenceId && 
                            (x.Type == VirtualCourtRoomType.Witness|| x.Type == VirtualCourtRoomType.Civilian))
                .ToListAsync();
        }
    }
}
