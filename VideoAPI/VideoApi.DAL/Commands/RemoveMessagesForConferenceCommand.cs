using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class RemoveMessagesForConferenceCommand : ICommand
    {
        public Guid ConferenceId { get; set; }
       
        public RemoveMessagesForConferenceCommand(Guid conferenceId)
        {
            ConferenceId = conferenceId;
        }
    }

    public class RemoveMessagesForConferenceCommandHandler : ICommandHandler<RemoveMessagesForConferenceCommand>
    {
        private readonly VideoApiDbContext _context;

        public RemoveMessagesForConferenceCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(RemoveMessagesForConferenceCommand command)
        {
            var conference = await _context.Conferences.Include("Messages")
                .SingleOrDefaultAsync(x => x.Id == command.ConferenceId);

            if (conference == null)
            {
                throw new ConferenceNotFoundException(command.ConferenceId);
            }

            conference.ClearMessages();
            await _context.SaveChangesAsync();
        }
    }
}
