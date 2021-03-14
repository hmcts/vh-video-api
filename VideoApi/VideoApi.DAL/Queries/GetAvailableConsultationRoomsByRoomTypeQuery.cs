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
    public class GetAvailableConsultationRoomsByRoomTypeQuery : IQuery
    {
        public VirtualCourtRoomType CourtRoomType { get; }
        public Guid ConferenceId { get; }

        public GetAvailableConsultationRoomsByRoomTypeQuery(VirtualCourtRoomType courtRoomType, Guid conferenceId)
        {
            CourtRoomType = courtRoomType;
            ConferenceId = conferenceId;
        }
    }

    public class GetAvailableConsultationRoomsByRoomTypeQueryHandler : IQueryHandler<GetAvailableConsultationRoomsByRoomTypeQuery, List<ConsultationRoom>>
    {
        private readonly VideoApiDbContext _context;

        public GetAvailableConsultationRoomsByRoomTypeQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<ConsultationRoom>> Handle(GetAvailableConsultationRoomsByRoomTypeQuery query)
        {
            var conference = await _context.Conferences.FindAsync(query.ConferenceId);
            
            if (conference == null)
            {
                throw new ConferenceNotFoundException(query.ConferenceId);
            }
            
            var rooms =  await _context.Rooms
                .Include(x => x.RoomParticipants)
                .Include(x => x.RoomEndpoints)
                .AsNoTracking()
                .Where(x => x.ConferenceId == query.ConferenceId && x.Type == query.CourtRoomType)
                .Where(x => x.Status == RoomStatus.Live)
                .ToListAsync();

            return rooms.Cast<ConsultationRoom>().ToList();
        }
    }
}
