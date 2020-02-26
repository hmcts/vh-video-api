using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class RemoveInstantMessagesForConferenceCommand : ICommand
    {
        public Guid ConferenceId { get; set; }
       
        public RemoveInstantMessagesForConferenceCommand(Guid conferenceId)
        {
            ConferenceId = conferenceId;
        }
    }

    public class RemoveMessagesForConferenceCommandHandler : ICommandHandler<RemoveInstantMessagesForConferenceCommand>
    {
        private readonly VideoApiDbContext _context;

        public RemoveMessagesForConferenceCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(RemoveInstantMessagesForConferenceCommand command)
        {
            var conference = await _context.Conferences.Include("InstantMessageHistory")
                .SingleOrDefaultAsync(x => x.Id == command.ConferenceId);

            if (conference == null)
            {
                throw new ConferenceNotFoundException(command.ConferenceId);
            }

            conference.ClearInstantMessageHistory();
            await _context.SaveChangesAsync();
        }
    }
}
