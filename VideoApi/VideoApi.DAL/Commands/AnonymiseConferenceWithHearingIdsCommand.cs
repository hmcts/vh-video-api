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
        AnonymiseConferenceWithHearingIdsCommandHandler : ICommandHandler<AnonymiseConferenceWithHearingIdsCommand>
    {
        private readonly VideoApiDbContext _context;

        public AnonymiseConferenceWithHearingIdsCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(AnonymiseConferenceWithHearingIdsCommand command)
        {
            var conferences = await _context.Conferences
                .Include(c => c.Participants)
                .Where(c => command.HearingIds.Contains(c.HearingRefId))
                .Distinct()
                .ToListAsync();

            if (!conferences.Any()) return;

            foreach (var conference in conferences.Where(conference =>
                         !conference.Participants
                             .Any(x => x.Username
                                 .Contains(Constants.AnonymisedUsernameSuffix)))
                    )
                conference.AnonymiseCaseName();

            await _context.SaveChangesAsync();
        }
    }
}
