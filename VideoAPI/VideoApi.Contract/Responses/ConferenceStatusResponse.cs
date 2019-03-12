using System;
using VideoApi.Domain.Enums;

namespace VideoApi.Contract.Responses
{
    public class ConferenceStatusResponse
    {
        public ConferenceState ConferenceState { get; set; }
        public DateTime TimeStamp { get; set; }
       
    }
}