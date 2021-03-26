using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class UpdateParticipantRoomConnectionDetailsCommand : ICommand
    {
        public UpdateParticipantRoomConnectionDetailsCommand(Guid conferenceId, long roomId, string label, string ingestUrl, string pexipNode,
            string participantUri)
        {
            ConferenceId = conferenceId;
            RoomId = roomId;
            Label = label;
            IngestUrl = ingestUrl;
            PexipNode = pexipNode;
            ParticipantUri = participantUri;
        }

        public Guid ConferenceId { get; }
        public long RoomId { get; }
        public string Label { get; }
        public string IngestUrl { get; }
        public string PexipNode { get; }
        public string ParticipantUri { get; }
    }

    public class UpdateParticipantRoomConnectionDetailsCommandHandler : ICommandHandler<UpdateParticipantRoomConnectionDetailsCommand>
    {
        private readonly VideoApiDbContext _context;

        public UpdateParticipantRoomConnectionDetailsCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateParticipantRoomConnectionDetailsCommand command)
        {
            var room = await _context.Rooms.OfType<ParticipantRoom>()
                .SingleOrDefaultAsync(x => x.ConferenceId == command.ConferenceId &&  x.Id == command.RoomId);

            if (room == null)
            {
                throw new RoomNotFoundException(command.ConferenceId, command.Label);
            }

            room.UpdateConnectionDetails(command.Label, command.IngestUrl, command.PexipNode, command.ParticipantUri);
            await _context.SaveChangesAsync();
        }
    }
}
