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

    public class GetConsultationRoomByIdQueryHandler : IQueryHandler<GetConsultationRoomByIdQuery, ConsultationRoom>
    {
        private readonly VideoApiDbContext _context;

        public GetConsultationRoomByIdQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public Task<ConsultationRoom> Handle(GetConsultationRoomByIdQuery query)
        {
            return _context.Rooms.OfType<ConsultationRoom>()
                .Include(x => x.RoomParticipants).ThenInclude(x => x.Participant)
                .Include(x => x.RoomEndpoints)
                .AsNoTracking()
                .Where(x => x.ConferenceId == query.ConferenceId && x.Label == query.RoomLabel)
                .FirstOrDefaultAsync();
        }
    }
}
