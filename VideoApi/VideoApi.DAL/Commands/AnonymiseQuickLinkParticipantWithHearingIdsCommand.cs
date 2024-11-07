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
        AnonymiseQuickLinkParticipantWithHearingIdsCommandHandler(VideoApiDbContext context) : ICommandHandler<
        AnonymiseQuickLinkParticipantWithHearingIdsCommand>
    {
        public async Task Handle(AnonymiseQuickLinkParticipantWithHearingIdsCommand command)
        {
            var conferences = await context.Conferences
                .Include(c => c.Participants)
                .Where(c => command.HearingIds.Contains(c.HearingRefId))
                .Distinct()
                .ToListAsync();

            if (conferences.Count == 0) return;

            foreach (var conference in conferences) conference.AnonymiseQuickLinkParticipants();

            await context.SaveChangesAsync();
        }
    }
}
