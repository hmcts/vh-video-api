using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;

namespace VideoApi.DAL.Commands
{
    public class UpdateParticipantUsernameCommand : ICommand
    {
        public Guid ParticipantId { get; set; }
        public string Username { get; set; }
        
        public UpdateParticipantUsernameCommand(Guid participantId, string username)
        {
            ParticipantId = participantId;
            Username = username;
        }
    }

    public class UpdateParticipantUsernameCommandHandler(VideoApiDbContext context) : ICommandHandler<UpdateParticipantUsernameCommand>
    {
        public async Task Handle(UpdateParticipantUsernameCommand command)
        {
            var conference = await context.Conferences
                .Include(x => x.Participants)
                .Include(x => x.Endpoints)
                .SingleOrDefaultAsync(x => x.Id == command.ParticipantId);
            
            var participant = conference
                .GetParticipants()
                .SingleOrDefault(x => x.Id == command.ParticipantId);
            
            if (participant == null)
                throw new ParticipantNotFoundException(command.ParticipantId);
            
            participant.UpdateUsername(command.Username);
            
            //Update any endpoints linked to this participant
            var endpoints = conference.GetEndpoints()?
                .Where(x => x.Id == command.ParticipantId)
                .ToList();
            
            if(endpoints != null && endpoints.Any())
                foreach (var endpoint in endpoints)
                    endpoint.AssignDefenceAdvocate(command.Username);
            
            await context.SaveChangesAsync();
        }
    }
}
