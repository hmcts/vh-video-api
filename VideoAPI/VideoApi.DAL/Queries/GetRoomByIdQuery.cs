using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;

namespace VideoApi.DAL.Queries
{
    public class GetRoomByIdQuery : IQuery
    {
        public long RoomId { get; set; }

        public GetRoomByIdQuery(long roomId)
        {
            RoomId = roomId;
        }
    }

    public class GetRoomByIdQueryHandler : IQueryHandler<GetRoomByIdQuery, Room>
    {
        private readonly VideoApiDbContext _context;

        public GetRoomByIdQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public Task<Room> Handle(GetRoomByIdQuery query)
        {
            return _context.Rooms
                .Include(r => r.RoomParticipants)
                .AsNoTracking()
                .SingleOrDefaultAsync(r => r.Id == query.RoomId);
        }
    }
}
