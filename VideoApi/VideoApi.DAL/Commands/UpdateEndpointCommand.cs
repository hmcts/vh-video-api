using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class UpdateEndpointCommand : ICommand
    {
        public Guid ConferenceId { get; }
        public string SipAddress { get; }
        public string DisplayName { get; }
        public (string, LinkedParticipantType)[] EndpointParticipants { get; }

        public UpdateEndpointCommand(Guid conferenceId,
            string sipAddress,
            string displayName,
            params (string Username, LinkedParticipantType Type)[] endpointParticipants)
        {
            ConferenceId = conferenceId;
            SipAddress = sipAddress;
            DisplayName = displayName;
            EndpointParticipants = endpointParticipants;
        }
    }

    public class UpdateEndpointCommandHandler : ICommandHandler<UpdateEndpointCommand>
    {
        private readonly VideoApiDbContext _context;
        public UpdateEndpointCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }
        public async Task Handle(UpdateEndpointCommand command)
        {
            var conference = await _context.Conferences.Include(x => x.Endpoints)
                .SingleOrDefaultAsync(x => x.Id == command.ConferenceId);

            if (conference == null)
                throw new ConferenceNotFoundException(command.ConferenceId);
            
            var endpoint = conference.GetEndpoints().SingleOrDefault(x => x.SipAddress == command.SipAddress);
            if (endpoint == null)
                throw new EndpointNotFoundException(command.SipAddress);

            if (!string.IsNullOrWhiteSpace(command.DisplayName)) 
                endpoint.UpdateDisplayName(command.DisplayName);

            if (command.EndpointParticipants.Any())
                endpoint.LinkParticipantsToEndpoint(command.EndpointParticipants);
            
            await _context.SaveChangesAsync();
        }
    }
}
