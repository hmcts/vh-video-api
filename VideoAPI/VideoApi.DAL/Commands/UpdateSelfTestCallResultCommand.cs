using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain.Enums;

namespace VideoApi.DAL.Commands
{
    public class UpdateSelfTestCallResultCommand : ICommand
    {
        public UpdateSelfTestCallResultCommand(Guid conferenceId, Guid participantId, bool passed, TestScore score)
        {
            ConferenceId = conferenceId;
            ParticipantId = participantId;
            Passed = passed;
            Score = score;
        }

        public Guid ConferenceId { get; set; }
        public Guid ParticipantId { get; set; }
        public bool Passed { get; set; }
        public TestScore Score { get; set; }
    }

    public class UpdateSelfTestCallResultCommandHandler : ICommandHandler<UpdateSelfTestCallResultCommand>
    {
        private readonly VideoApiDbContext _context;

        public UpdateSelfTestCallResultCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateSelfTestCallResultCommand command)
        {
            var conference = await _context.Conferences.Include("Participants.TestCallResult")
                .SingleOrDefaultAsync(x => x.Id == command.ConferenceId);

            if (conference == null)
            {
                throw new ConferenceNotFoundException(command.ConferenceId);
            }

            var participant = conference.GetParticipants().SingleOrDefault(x => x.Id == command.ParticipantId);
            if (participant == null)
            {
                throw new ParticipantNotFoundException(command.ParticipantId);
            }
            
            participant.UpdateTestCallResult(command.Passed, command.Score);
            await _context.SaveChangesAsync();
        }
    }
}