using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;

namespace VideoApi.DAL.Queries
{
    public class GetConferenceByIdQuery : IQuery
    {
        public Guid ConferenceId { get; set; }

        public GetConferenceByIdQuery(Guid conferenceId)
        {
            ConferenceId = conferenceId;
        }
    }

    public class GetConferenceByIdQueryHandler : IQueryHandler<GetConferenceByIdQuery, Conference>
    {
        private readonly VideoApiDbContext _context;

        public GetConferenceByIdQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task<Conference> Handle(GetConferenceByIdQuery query)
        {
            var conference = await _context.Conferences
                .Include(x => x.Participants).ThenInclude(x => x.LinkedParticipants)
                .Include(x => x.Endpoints)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == query.ConferenceId);

            if (conference == null)
            {
                return null;
            }

            var rooms = await _context.Rooms
                .Include(x => x.RoomParticipants)
                .Include(x => x.RoomEndpoints)
                .AsNoTracking().Where(x => x.ConferenceId == query.ConferenceId).ToListAsync();

            // Rooms is set as readonly list so cannot add rooms directly to it.
            conference.AddRooms(rooms);

            foreach (var participant in conference.Participants)
            {
                participant.CurrentConsultationRoom = rooms.SingleOrDefault(r => r.Id == participant.CurrentConsultationRoomId) as ConsultationRoom;
                
                // A participant can be in more than one room as there is no room participant concept implemented
                // i.e. room as a participant joining the call
                var participantRooms = rooms.Where(r => r.RoomParticipants.Any(x => x.ParticipantId == participant.Id));
                foreach (var room in participantRooms)
                {
                    foreach (var roomParticipant in room.RoomParticipants.Where(x => x.ParticipantId == participant.Id))
                    {
                        roomParticipant.Room = room;
                        participant.RoomParticipants.Add(roomParticipant);
                        roomParticipant.Participant = participant;
                    }
                }
            }

            foreach (var endpoint in conference.Endpoints)
            {
                endpoint.CurrentConsultationRoom = rooms.SingleOrDefault(r => r.Id == endpoint.CurrentConsultationRoomId) as ConsultationRoom;
            }

            return conference;
        }
    }
}
