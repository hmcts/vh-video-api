using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class AnonymiseConferenceWithHearingIdsCommand : ICommand
    {
        public List<Guid> HearingIds { get; set; }
    }

    public class
        AnonymiseConferenceWithHearingIdsCommandHandler(VideoApiDbContext context)
        : ICommandHandler<AnonymiseConferenceWithHearingIdsCommand>
    {
        public async Task Handle(AnonymiseConferenceWithHearingIdsCommand command)
        {
            var conferences = await context.Conferences
                .Include(c => c.Participants)
                .Where(c => command.HearingIds.Contains(c.HearingRefId))
                .Distinct()
                .ToListAsync();

            if (conferences.Count == 0) return;

            foreach (var conference in conferences.Where(conference =>
                         !conference.Participants
                             .Any(x => x.Username
                                 .Contains(Constants.AnonymisedUsernameSuffix)))
                    )
                conference.AnonymiseCaseName();

            await context.SaveChangesAsync();
        }
    }
}
