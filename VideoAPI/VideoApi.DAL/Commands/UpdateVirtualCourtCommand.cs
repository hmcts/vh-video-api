using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;

namespace VideoApi.DAL.Commands
{
    public class UpdateVirtualCourtCommand : ICommand
    {
        public UpdateVirtualCourtCommand(Guid conferenceId, string adminUri, string judgeUri, string participantUri,
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

    public class UpdateVirtualCourtCommandHandler : ICommandHandler<UpdateVirtualCourtCommand>
    {
        private readonly VideoApiDbContext _context;

        public UpdateVirtualCourtCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateVirtualCourtCommand command)
        {
            var conference = await _context.Conferences.Include(x => x.VirtualCourt)
                .SingleOrDefaultAsync(x => x.Id == command.ConferenceId);

            if (conference == null)
            {
                throw new ConferenceNotFoundException(command.ConferenceId);
            }

            conference.UpdateVirtualCourt(command.AdminUri, command.JudgeUri, command.ParticipantUri,
                command.PexipNode);
            await _context.SaveChangesAsync();
        }
    }
}