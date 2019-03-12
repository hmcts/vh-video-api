using System;
using VideoApi.Domain.Enums;

namespace VideoApi.Contract.Responses
{
    public class ParticipantStatusResponse
    {
        public ParticipantState ParticipantState { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}