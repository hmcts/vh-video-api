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
            string pexipNode, string telephoneConferenceId)
        {
            ConferenceId = conferenceId;
            AdminUri = adminUri;
            JudgeUri = judgeUri;
            ParticipantUri = participantUri;
            PexipNode = pexipNode;
            TelephoneConferenceId = telephoneConferenceId;
        }

        public Guid ConferenceId { get; }
        public string AdminUri { get; }
        public string JudgeUri { get; }
        public string ParticipantUri { get; }
        public string PexipNode { get; }
        public string TelephoneConferenceId { get; }
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

            conference.UpdateMeetingRoom(command.AdminUri, command.JudgeUri, command.ParticipantUri, command.PexipNode,
                command.TelephoneConferenceId);
            
            await _context.SaveChangesAsync();
        }
    }
}
