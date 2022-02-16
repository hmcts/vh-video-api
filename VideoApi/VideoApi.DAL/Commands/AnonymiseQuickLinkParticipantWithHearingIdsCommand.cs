using System;
using System.Collections.Generic;
using System.Linq;
using RandomStringCreator;
using VideoApi.DAL.Commands.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class AnonymiseQuickLinkParticipantWithHearingIdsCommand : ICommand
    {
        public List<Guid> HearingIds;
    }

    public class
        AnonymiseQuickLinkParticipantWithHearingIdsCommandHandler : ICommandHandler<
            AnonymiseQuickLinkParticipantWithHearingIdsCommand>
    {
        private readonly VideoApiDbContext _context;

        public AnonymiseQuickLinkParticipantWithHearingIdsCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(AnonymiseQuickLinkParticipantWithHearingIdsCommand command)
        {
            var conferences = _context.Conferences
                .Where(c => command.HearingIds.Contains(c.HearingRefId))
                .Distinct()
                .ToList();

            var quickLinkParticipants = (
                from conference in conferences
                join participant in _context.Participants on conference.Id equals participant.ConferenceId
                where (participant.UserRole == UserRole.QuickLinkObserver ||
                       participant.UserRole == UserRole.QuickLinkParticipant) &&
                      !participant.Username.Contains(Constants.AnonymisedUsernameSuffix)
                select participant).ToList();

            var anonymisedParticipants = quickLinkParticipants.Select(participant => AnonymiseParticipant(participant)).ToList();

            _context.Participants.UpdateRange(anonymisedParticipants);

            await _context.SaveChangesAsync();
        }

        private Participant AnonymiseParticipant(Participant participant)
        {
            var randomString = new StringCreator().Get(9).ToLowerInvariant();

            participant.Username = $"{randomString}{Constants.AnonymisedUsernameSuffix}";
            participant.Name = $"{randomString} {randomString}";
            participant.DisplayName = randomString;

            return participant;
        }
    }
}
