using System.Linq;
using Microsoft.EntityFrameworkCore;
using RandomStringCreator;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class AnonymiseParticipantWithUsernameCommand : ICommand
    {
        public string Username { get; set; }
    }

    public class
        AnonymiseParticipantWithUsernameCommandHandler : ICommandHandler<AnonymiseParticipantWithUsernameCommand>
    {
        private readonly VideoApiDbContext _context;

        public AnonymiseParticipantWithUsernameCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(AnonymiseParticipantWithUsernameCommand command)
        {
            var participantsToAnonymise = await _context.Participants
                .Where(p => p.Username == command.Username)
                .ToListAsync();

            if (participantsToAnonymise.Count < 1)
            {
                throw new ParticipantNotFoundException(command.Username);
            }
            
            var processedParticipants = (
                    from participant
                        in participantsToAnonymise
                    where !participant.Username.Contains(Constants.AnonymisedUsernameSuffix)
                    select AnonymiseParticipant(participant))
                .ToList();

            _context.Participants.UpdateRange(processedParticipants);

            await _context.SaveChangesAsync();
        }

        private Participant AnonymiseParticipant(Participant participant)
        {
            var randomString = new StringCreator().Get(9).ToUpperInvariant();

            participant.Name = randomString;
            participant.DisplayName = $"{randomString} {randomString}";
            participant.FirstName = randomString;
            participant.LastName = randomString;
            participant.ContactEmail = randomString;
            participant.ContactTelephone = randomString;
            participant.Username = $"{randomString}{Constants.AnonymisedUsernameSuffix}";

            if (!string.IsNullOrWhiteSpace(participant.Representee)) participant.Representee = randomString;

            return participant;
        }
    }
}
