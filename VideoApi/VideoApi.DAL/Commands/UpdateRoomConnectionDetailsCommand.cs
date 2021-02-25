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

        public async Task Handle(UpdateRoomConnectionDetailsCommand connectionDetailsCommand)
        {
            var room = await _context.Rooms
                .SingleOrDefaultAsync(x => x.ConferenceId == connectionDetailsCommand.ConferenceId &&  x.Id == connectionDetailsCommand.RoomId);

            if (room == null)
            {
                throw new RoomNotFoundException(connectionDetailsCommand.ConferenceId, connectionDetailsCommand.Label);
            }

            room.UpdateRoomConnectionDetails(connectionDetailsCommand.Label, connectionDetailsCommand.IngestUrl, connectionDetailsCommand.PexipNode, connectionDetailsCommand.ParticipantUri);
            await _context.SaveChangesAsync();
        }
    }
}
