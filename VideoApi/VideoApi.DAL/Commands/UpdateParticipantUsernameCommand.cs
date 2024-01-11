using System;
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
            var participant = await _context.Participants
                .SingleOrDefaultAsync(x => x.Id == command.ParticipantId);
            
            if (participant == null)
            {
                throw new ParticipantNotFoundException(command.ParticipantId);
            }

            participant.UpdateUsername(command.Username);
            await _context.SaveChangesAsync();
        }
    }
}
