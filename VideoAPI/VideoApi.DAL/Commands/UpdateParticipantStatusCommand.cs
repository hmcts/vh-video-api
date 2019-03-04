using System.Threading.Tasks;
using VideoApi.Domain.Enums;

namespace VideoApi.DAL.Commands
{
    public class UpdateParticipantStatusCommand : ICommand
    {
        public long ParticipantId { get; set; }
        public ParticipantState ParticipantState { get; set; }

        public UpdateParticipantStatusCommand(long participantId, ParticipantState participantState)
        {
            ParticipantId = participantId;
            ParticipantState = participantState;
        }
    }

    public class UpdateParticipantStatusCommandHandler : ICommandHandler<UpdateParticipantStatusCommand>
    {
        private readonly VideoApiDbContext _context;
        
        public UpdateParticipantStatusCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }
        
        public Task Handle(UpdateParticipantStatusCommand command)
        {
            throw new System.NotImplementedException();
        }
    }
}