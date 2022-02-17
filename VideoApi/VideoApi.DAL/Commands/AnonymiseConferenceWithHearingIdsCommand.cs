using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;

namespace VideoApi.DAL.Commands
{
    public class AnonymiseConferenceWithHearingIdsCommand : ICommand
    {
        public List<Guid> HearingIds;
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
            var conferences = _context.Conferences
                .Where(c => command.HearingIds.Contains(c.HearingRefId))
                .Distinct()
                .ToList();

            if (conferences.Count < 1)
            {
                throw new ConferenceNotFoundException(command.HearingIds);
            }
            
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
