using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;

namespace VideoApi.DAL.Commands
{
    public class AnonymiseQuickLinkParticipantWithHearingIdsCommand : ICommand
    {
        public List<Guid> HearingIds { get; set; }
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

            if (!conferences.Any()) return;

            foreach (var conference in conferences) conference.AnonymiseQuickLinkParticipants();

            await _context.SaveChangesAsync();
        }
    }
}
