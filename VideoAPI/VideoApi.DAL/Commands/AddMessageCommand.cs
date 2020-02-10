using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;

namespace VideoApi.DAL.Commands
{
    public class AddMessageCommand : ICommand
    {
        public AddMessageCommand(Guid conferenceId, string from, string to, string messageText)
        {
            ConferenceId = conferenceId;
            From = from;
            To = to;
            MessageText = messageText;
        }

        public string From { get; set; }
        public string To { get; set; }
        public string MessageText { get; set; }
        public Guid ConferenceId { get; }
    }

    public class AddMessageCommandHandler : ICommandHandler<AddMessageCommand>
    {
        private readonly VideoApiDbContext _context;

        public AddMessageCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(AddMessageCommand command)
        {
            var conference = await _context.Conferences
                                    .Include(x => x.Messages)
                                    .SingleOrDefaultAsync(x => x.Id == command.ConferenceId);

            if (conference == null)
            {
                throw new ConferenceNotFoundException(command.ConferenceId);
            }

            conference.AddMessage(command.From, command.To, command.MessageText);
            await _context.SaveChangesAsync();
        }
    }
}
