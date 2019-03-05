using VideoApi.Domain.Enums;

namespace VideoApi.Contract.Requests
{
    public class UpdateParticipantStatusRequest
    {
        public ParticipantState State { get; set; }
    }
}