using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;

namespace VideoApi.DAL.Commands
{
    public class UpdateRoomConnectionDetailsCommand : ICommand
    {
        public UpdateRoomConnectionDetailsCommand(Guid conferenceId, long roomId, string label, string ingestUrl, string pexipNode,
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

    public class UpdateRoomConnectionDetailsCommandHandler : ICommandHandler<UpdateRoomConnectionDetailsCommand>
    {
        private readonly VideoApiDbContext _context;

        public UpdateRoomConnectionDetailsCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateRoomConnectionDetailsCommand command)
        {
            var room = await _context.Rooms
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
