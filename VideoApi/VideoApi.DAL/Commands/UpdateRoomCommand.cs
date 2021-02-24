using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;

namespace VideoApi.DAL.Commands
{
    public class UpdateRoomCommand : ICommand
    {
        public UpdateRoomCommand(Guid conferenceId, long roomId, string label, string ingestUrl, string pexipNode,
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

    public class UpdateRoomCommandHandler : ICommandHandler<UpdateRoomCommand>
    {
        private readonly VideoApiDbContext _context;

        public UpdateRoomCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateRoomCommand command)
        {
            var room = await _context.Rooms
                .SingleOrDefaultAsync(x => x.ConferenceId == command.ConferenceId &&  x.Id == command.RoomId);

            if (room == null)
            {
                throw new RoomNotFoundException(command.ConferenceId, command.Label);
            }

            room.AddRoomConnectionDetails(command.Label, command.IngestUrl, command.PexipNode, command.ParticipantUri);
            await _context.SaveChangesAsync();
        }
    }
}
