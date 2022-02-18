using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RandomStringCreator;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
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
                .Include(c => c.Participants)
                .Where(c => command.HearingIds.Contains(c.HearingRefId))
                .Distinct()
                .ToList();

            if (conferences.Count < 1)
            {
                throw new ConferenceNotFoundException(command.HearingIds);
            }

            foreach (var conference in conferences)
            {
                conference.AnonymiseQuickLinkParticipants();
            }

            await _context.SaveChangesAsync();
        }
    }
}
