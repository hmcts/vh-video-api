using System;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class AddEndpointCommand : ICommand
    {
        public Guid ConferenceId { get; }
        public string DisplayName { get; }
        public string SipAddress { get; }
        public string Pin { get; }
        public string DefenceAdvocate { get; }

        public AddEndpointCommand(Guid conferenceId, string displayName, string sipAddress, string pin,
            string defenceAdvocate)
        {
            ConferenceId = conferenceId;
            DisplayName = displayName;
            SipAddress = sipAddress;
            Pin = pin;
            DefenceAdvocate = defenceAdvocate;
        }
    }

    public class AddEndpointCommandHandler : ICommandHandler<AddEndpointCommand>
    {
        private readonly VideoApiDbContext _context;

        public AddEndpointCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(AddEndpointCommand command)
        {
            var conference = await _context.Conferences.Include(x => x.Endpoints)
                .SingleOrDefaultAsync(x => x.Id == command.ConferenceId);

            if (conference == null)
            {
                throw new ConferenceNotFoundException(command.ConferenceId);
            }

            var ep = new Endpoint(command.DisplayName, command.SipAddress, command.Pin, command.DefenceAdvocate);
            conference.AddEndpoint(ep);
            _context.Entry(ep).State = EntityState.Added;
            await _context.SaveChangesAsync();
        }
    }
}
