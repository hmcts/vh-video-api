using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;

namespace VideoApi.DAL.Commands
{
    public class UpdateParticipantUsernameCommand(Guid participantId, string username) : ICommand
    {
        public Guid ParticipantId { get; set; } = participantId;
        public string Username { get; set; } = username;
    }

    public class UpdateParticipantUsernameCommandHandler(VideoApiDbContext context)
        : ICommandHandler<UpdateParticipantUsernameCommand>
    {
        public async Task Handle(UpdateParticipantUsernameCommand command)
        {
            var conference = await context.Conferences
                .Include(x => x.Participants)
                .Include(x => x.Endpoints)
                .SingleOrDefaultAsync(x => x.Participants.Any(p => p.Id == command.ParticipantId));
              
            if (conference == null)
                throw new ParticipantNotFoundException(command.ParticipantId);
            
            var participant = conference.GetParticipants().Single(x => x.Id == command.ParticipantId);
            
            //Find any endpoints linked to this participant
            var endpoints = conference.GetEndpoints()?
                .Where(x => x.DefenceAdvocate == participant.Username)
                .ToList();
            
            //Update username and endpoint defence advocates with that username
            participant.UpdateUsername(command.Username);
            
            if(endpoints != null && endpoints.Count != 0)
                foreach (var endpoint in endpoints)
                    endpoint.AssignDefenceAdvocate(command.Username);
            
            await context.SaveChangesAsync();
        }
    }
}
