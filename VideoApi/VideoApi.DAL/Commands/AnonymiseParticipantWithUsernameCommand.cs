﻿using System.Linq;
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
        public const string AnonymisedUsernameSuffix = "@email.net";
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
            
            var processedParticipants = (
                    from participant
                        in participantsToAnonymise
                    where !participant.Username.Contains(AnonymisedUsernameSuffix)
                    select AnonymiseParticipant(participant))
                .ToList();

            _context.Participants.UpdateRange(processedParticipants);

            await _context.SaveChangesAsync();
        }

        private Participant AnonymiseParticipant(Participant participant)
        {
            var randomString = new StringCreator().Get(9).ToLower();

            participant.Name = randomString;
            participant.DisplayName = $"{randomString} {randomString}";
            participant.FirstName = randomString;
            participant.LastName = randomString;
            participant.ContactEmail = randomString;
            participant.ContactTelephone = randomString;
            participant.Username = $"{randomString}{AnonymisedUsernameSuffix}";

            if (!string.IsNullOrWhiteSpace(participant.Representee)) participant.Representee = randomString;

            return participant;
        }
    }
}
