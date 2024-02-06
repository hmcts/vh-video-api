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

    public class UpdateParticipantUsernameCommandHandler : ICommandHandler<UpdateParticipantUsernameCommand>
    {
        private readonly VideoApiDbContext _context;

        public UpdateParticipantUsernameCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateParticipantUsernameCommand command)
        {
            var conference = await _context.Conferences
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
            
            if(endpoints != null && endpoints.Any())
                foreach (var endpoint in endpoints)
                    endpoint.AssignDefenceAdvocate(command.Username);
            
            await _context.SaveChangesAsync();
        }
    }
}
