using VideoApi.Domain.Enums;

namespace VideoApi.Contract.Responses
{
    public class ParticipantSummaryResponse
    {
        public string Username { get; set; }
        public ParticipantState Status { get; set; }
    }
}