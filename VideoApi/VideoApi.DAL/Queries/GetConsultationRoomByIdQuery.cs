using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;

namespace VideoApi.DAL.Queries
{
    public class GetConsultationRoomByIdQuery : IQuery
    {
        public GetConsultationRoomByIdQuery(Guid conferenceId, string roomLabel)
        {
            ConferenceId = conferenceId;
            RoomLabel = roomLabel;
        }

        public Guid ConferenceId { get; }

        public string RoomLabel { get; }
    }

    public class GetRoomByIdQueryHandler : IQueryHandler<GetConsultationRoomByIdQuery, ConsultationRoom>
    {
        private readonly VideoApiDbContext _context;

        public GetRoomByIdQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task<ConsultationRoom> Handle(GetConsultationRoomByIdQuery query)
        {
            var room = await _context.Rooms
                .Include(x => x.RoomParticipants)
                .Include(x => x.RoomEndpoints)
                .AsNoTracking()
                .Where(x => x.ConferenceId == query.ConferenceId && x.Label == query.RoomLabel
                && x is ConsultationRoom)
                .FirstOrDefaultAsync();

            return (ConsultationRoom)room;
        }
    }
}
