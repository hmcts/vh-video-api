using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.DAL.Queries
{
    public class GetAvailableRoomByRoomTypeQuery : IQuery
    {
        public VirtualCourtRoomType CourtRoomType { get; }
        public Guid ConferenceId { get; }

        public GetAvailableRoomByRoomTypeQuery(VirtualCourtRoomType courtRoomType, Guid conferenceId)
        {
            CourtRoomType = courtRoomType;
            ConferenceId = conferenceId;
        }
    }

    public class GetAvailableRoomByRoomTypeQueryHandler : IQueryHandler<GetAvailableRoomByRoomTypeQuery, List<Room>>
    {
        private readonly VideoApiDbContext _context;

        public GetAvailableRoomByRoomTypeQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<Room>> Handle(GetAvailableRoomByRoomTypeQuery query)
        {
            var conference = await _context.Conferences.FindAsync(query.ConferenceId);
            
            if (conference == null)
            {
                throw new ConferenceNotFoundException(query.ConferenceId);
            }
            
            return await _context.Rooms
                .Include(x => x.RoomParticipants)
                .AsNoTracking()
                .Where(x => x.ConferenceId == query.ConferenceId && x.Type == query.CourtRoomType)
                .Where(x => x.Status == RoomStatus.Live)
                .ToListAsync();
        }
    }
}
