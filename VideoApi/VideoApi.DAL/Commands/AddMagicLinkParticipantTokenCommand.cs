using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VideoApi.Common.Security;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class AddMagicLinkParticipantTokenCommand : ICommand
    {
        public AddMagicLinkParticipantTokenCommand(Guid participantId, MagicLinksJwtDetails jwtDetails)
        {
            ParticipantId = participantId;
            JwtDetails = jwtDetails;
        }
        
        public Guid ParticipantId { get; }
        public MagicLinksJwtDetails JwtDetails { get; }
    }
    
    public class AddMagicLinkParticipantTokenCommandHandler : ICommandHandler<AddMagicLinkParticipantTokenCommand>
    {
        private readonly VideoApiDbContext _context;

        public AddMagicLinkParticipantTokenCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(AddMagicLinkParticipantTokenCommand command)
        {
            await _context.ParticipantTokens.AddAsync(new ParticipantToken()
            {
                Jwt = command.JwtDetails.Token,
                ExpiresAt = command.JwtDetails.Expiry,
                ParticipantId = command.ParticipantId
            });

            await _context.SaveChangesAsync();
        }
    }
}
