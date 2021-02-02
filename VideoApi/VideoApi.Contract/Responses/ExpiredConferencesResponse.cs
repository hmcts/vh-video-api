using System;
using VideoApi.Domain.Enums;

namespace VideoApi.Contract.Responses
{
    public class ExpiredConferencesResponse
    {
        /// <summary>
        /// The conference's UUID
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// The current conference status
        /// </summary>
        public ConferenceState CurrentStatus { get; set; }

        /// <summary>
        /// The hearing Id
        /// </summary>
        public Guid HearingId { get; set; }
    }
}
