using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using VideoApi.DAL.Commands.Core;
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
            var instantMessages = await _context.InstantMessages.Where(x => x.ConferenceId == command.ConferenceId).ToListAsync();

            _context.RemoveRange(instantMessages);
            await _context.SaveChangesAsync();
        }
    }
}
