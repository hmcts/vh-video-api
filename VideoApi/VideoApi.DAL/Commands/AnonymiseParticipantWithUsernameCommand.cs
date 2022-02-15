using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RandomStringCreator;
using VideoApi.DAL.Commands.Core;
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
        public const string AnonymisedUsernameSuffix = "@email.net";

        public AnonymiseParticipantWithUsernameCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(AnonymiseParticipantWithUsernameCommand command)
        {
            // var allParticipants = _context.Participants.ToList();
            var participantsToAnonymise = _context.Participants
                .Where(p => p.Username == command.Username)
                .ToList();
            // var participantsToAnonymise = _context.Participants
            //     .Where(p => p.Username.Contains(command.Username))
            //     .ToList();
            // var participantsToAnonymise = allParticipants
            //     .Where(p => p.Username == command.Username)
            //     .ToList();
               
            var processedParticipants = new List<ParticipantBase>();

            foreach (var participant in participantsToAnonymise)
            {
                var processedParticipant = AnonymiseParticipant(participant);
                processedParticipants.Add(processedParticipant);
            }
            
            await _context.SaveChangesAsync();
        }

        private Participant AnonymiseParticipant(Participant participant)
        {
            var randomString = new StringCreator().Get(9);

            participant.Name = randomString;
            participant.DisplayName = randomString;
            participant.FirstName = randomString;
            participant.LastName = randomString;
            participant.ContactEmail = randomString;
            participant.ContactTelephone = randomString;
            participant.Username = $"{randomString}{AnonymisedUsernameSuffix}";

            return participant;
        }
    }
}
