using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class UpdateInterpreterRoomConnectionDetailsCommand : ICommand
    {
        public UpdateInterpreterRoomConnectionDetailsCommand(Guid conferenceId, long roomId, string label, string ingestUrl, string pexipNode,
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

    public class UpdateInterpreterRoomConnectionDetailsCommandHandler : ICommandHandler<UpdateInterpreterRoomConnectionDetailsCommand>
    {
        private readonly VideoApiDbContext _context;

        public UpdateInterpreterRoomConnectionDetailsCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateInterpreterRoomConnectionDetailsCommand command)
        {
            var room = await _context.Rooms.OfType<InterpreterRoom>()
                .SingleOrDefaultAsync(x => x.ConferenceId == command.ConferenceId &&  x.Id == command.RoomId);

            if (room == null)
            {
                throw new RoomNotFoundException(command.ConferenceId, command.Label);
            }

            room.UpdateRoomConnectionDetails(command.Label, command.IngestUrl, command.PexipNode, command.ParticipantUri);
            await _context.SaveChangesAsync();
        }
    }
}
