using System;
using VideoApi.Common.Security;
using VideoApi.DAL.Commands.Core;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class AddQuickLinkParticipantTokenCommand : ICommand
    {
        public AddQuickLinkParticipantTokenCommand(Guid participantId, QuickLinksJwtDetails jwtDetails)
        {
            ParticipantId = participantId;
            JwtDetails = jwtDetails;
        }
        
        public Guid ParticipantId { get; }
        public QuickLinksJwtDetails JwtDetails { get; }
    }
    
    public class AddQuickLinkParticipantTokenCommandHandler : ICommandHandler<AddQuickLinkParticipantTokenCommand>
    {
        private readonly VideoApiDbContext _context;

        public AddQuickLinkParticipantTokenCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(AddQuickLinkParticipantTokenCommand command)
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
