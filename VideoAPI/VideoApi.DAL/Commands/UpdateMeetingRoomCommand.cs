using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;

namespace VideoApi.DAL.Commands
{
    public class UpdateMeetingRoomCommand : ICommand
    {
        public UpdateMeetingRoomCommand(Guid conferenceId, string adminUri, string judgeUri, string participantUri,
            string pexipNode)
        {
            ConferenceId = conferenceId;
            AdminUri = adminUri;
            JudgeUri = judgeUri;
            ParticipantUri = participantUri;
            PexipNode = pexipNode;
        }

        public Guid ConferenceId { get; set; }
        public string AdminUri { get; set; }
        public string JudgeUri { get; set; }
        public string ParticipantUri { get; set; }
        public string PexipNode { get; set; }
    }

    public class UpdateMeetingRoomHandler : ICommandHandler<UpdateMeetingRoomCommand>
    {
        private readonly VideoApiDbContext _context;

        public UpdateMeetingRoomHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateMeetingRoomCommand command)
        {
            var conference = await _context.Conferences.SingleOrDefaultAsync(x => x.Id == command.ConferenceId);

            if (conference == null)
            {
                throw new ConferenceNotFoundException(command.ConferenceId);
            }

            conference.UpdateMeetingRoom(command.AdminUri, command.JudgeUri, command.ParticipantUri,
                command.PexipNode);
            await _context.SaveChangesAsync();
        }
    }
}