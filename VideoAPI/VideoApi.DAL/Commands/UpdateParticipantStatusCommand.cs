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
}