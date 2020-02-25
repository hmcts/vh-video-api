using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;

namespace VideoApi.DAL.Commands
{
    public class AddInstantMessageCommand : ICommand
    {
        public AddInstantMessageCommand(Guid conferenceId, string from, string messageText)
        {
            ConferenceId = conferenceId;
            From = from;
            MessageText = messageText;
        }

        public string From { get; set; }
        public string MessageText { get; set; }
        public Guid ConferenceId { get; }
    }

    public class AddInstantMessageCommandHandler : ICommandHandler<AddInstantMessageCommand>
    {
        private readonly VideoApiDbContext _context;

        public AddInstantMessageCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(AddInstantMessageCommand command)
        {
            var conference = await _context.Conferences
                .Include(x => x.Participants)
                .Include(x => x.InstantMessageHistory)
                .SingleOrDefaultAsync(x => x.Id == command.ConferenceId);

            if (conference == null)
            {
                throw new ConferenceNotFoundException(command.ConferenceId);
            }

            conference.AddInstantMessage(command.From, command.MessageText);
            await _context.SaveChangesAsync();
        }
    }
}
