using System.Linq;
using RandomStringCreator;
using VideoApi.DAL.Commands.Core;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class AnonymiseParticipantWithUsernameCommand : ICommand
    {
        public string Username { get; init; }
    }

    public class
        AnonymiseParticipantWithUsernameCommandHandler(VideoApiDbContext context)
        : ICommandHandler<AnonymiseParticipantWithUsernameCommand>
    {
        public async Task Handle(AnonymiseParticipantWithUsernameCommand command)
        {
            var participantsToAnonymise = context.Participants
                                                  .AsEnumerable()
                                                  .Where(p => p.Username == command.Username)
                                                  .ToList();

            if (participantsToAnonymise.Count == 0) return;

            var processedParticipants = (
                    from participant
                        in participantsToAnonymise
                    where !participant.Username.Contains(Constants.AnonymisedUsernameSuffix)
                    select AnonymiseParticipant(participant))
                .ToList();

            context.Participants.UpdateRange(processedParticipants);

            await context.SaveChangesAsync();
        }

        private static Participant AnonymiseParticipant(Participant participant)
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
