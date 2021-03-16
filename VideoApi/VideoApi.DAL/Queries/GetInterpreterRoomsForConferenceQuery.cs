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
    public class GetInterpreterRoomsForConferenceQuery : IQuery
    {
        public GetInterpreterRoomsForConferenceQuery(Guid conferenceId)
        {
            ConferenceId = conferenceId;
        }

        public Guid ConferenceId { get; }
    }
    
    public class GetInterpreterRoomsForConferenceQueryHandler : IQueryHandler<GetInterpreterRoomsForConferenceQuery, List<InterpreterRoom>>
    {
        private readonly VideoApiDbContext _context;

        public GetInterpreterRoomsForConferenceQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public Task<List<InterpreterRoom>> Handle(GetInterpreterRoomsForConferenceQuery query)
        {
            return _context.Rooms.OfType<InterpreterRoom>()
                .Include(x => x.RoomParticipants)
                .AsNoTracking()
                .Where(x => x.ConferenceId == query.ConferenceId && 
                            (x.Type == VirtualCourtRoomType.Witness|| x.Type == VirtualCourtRoomType.Civilian))
                .ToListAsync();
        }
    }
}
