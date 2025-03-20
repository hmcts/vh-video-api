using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class UpdateEndpointCommand(
        Guid conferenceId,
        string sipAddress,
        string displayName,
        List<string> participantsLinked,
        ConferenceRole conferenceRole)
        : ICommand
    {
        public Guid ConferenceId { get; } = conferenceId;
        public string SipAddress { get; } = sipAddress;
        public string DisplayName { get; } = displayName;
        public IList<string> ParticipantsLinked { get; } = participantsLinked;
        public ConferenceRole ConferenceRole { get; } = conferenceRole;
    }

    public class UpdateEndpointCommandHandler(VideoApiDbContext context) : ICommandHandler<UpdateEndpointCommand>
    {
        public async Task Handle(UpdateEndpointCommand command)
        {
            var conference = await context.Conferences.Include(x => x.Endpoints)
                .SingleOrDefaultAsync(x => x.Id == command.ConferenceId);

            if (conference == null)
                throw new ConferenceNotFoundException(command.ConferenceId);
            
            var endpoint = conference.GetEndpoints().SingleOrDefault(x => x.SipAddress == command.SipAddress);
            if (endpoint == null)
                throw new EndpointNotFoundException(command.SipAddress);

            if (!string.IsNullOrWhiteSpace(command.DisplayName)) 
                endpoint.UpdateDisplayName(command.DisplayName);
            
            UpdateParticipantsLinkedToEndpoint(command, conference, endpoint);
            
            endpoint.UpdateConferenceRole(command.ConferenceRole);
            await context.SaveChangesAsync();
        }
        
        private static void UpdateParticipantsLinkedToEndpoint(UpdateEndpointCommand command, Conference conference,
            Endpoint endpoint)
        {
            var participants = conference.GetParticipants();
            var currentLinkedParticipants = endpoint.ParticipantsLinked.Select(p => p.Username).ToList();
            
            // Remove participants that are no longer linked
            foreach (var participant in endpoint.ParticipantsLinked.Where(p => !command.ParticipantsLinked.Contains(p.Username)).ToList())
                endpoint.RemoveParticipantLink(participant);
            
            // Add new participants
            foreach (var newParticipantLink in command.ParticipantsLinked.Except(currentLinkedParticipants))
            {
                var participant = participants.SingleOrDefault(p => p.Username == newParticipantLink) ?? throw new ParticipantNotFoundException(newParticipantLink);
                endpoint.AddParticipantLink(participant);
            }
        }
    }
}
