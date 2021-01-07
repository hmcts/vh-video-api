using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.DAL.Queries
{
    public class GetAvailableRoomByRoomTypeQuery : IQuery
    {
        public VirtualCourtRoomType CourtRoomType { get; }

        public GetAvailableRoomByRoomTypeQuery(VirtualCourtRoomType courtRoomType)
        {
            CourtRoomType = courtRoomType;
        }
    }

    public class GetAvailableRoomByRoomTypeQueryHandler : IQueryHandler<GetAvailableRoomByRoomTypeQuery, List<Room>>
    {
        private readonly VideoApiDbContext _context;

        public GetAvailableRoomByRoomTypeQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public Task<List<Room>> Handle(GetAvailableRoomByRoomTypeQuery query)
        {
            return _context.Rooms
                .Include(x => x.RoomParticipants)
                .AsNoTracking()
                .Where(x => x.Type == query.CourtRoomType)
                .Where(x => x.Status == RoomStatus.Live || x.Status == RoomStatus.Created)
                .ToListAsync();
        }
      
    }
}
