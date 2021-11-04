using System;
using VideoApi.Contract.Enums;

namespace VideoApi.Contract.Responses
{
    public class ConferenceForIndividualResponse
    {
        /// <summary>
        /// Conference UUID
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Scheduled date time as UTC
        /// </summary>
        public DateTime ScheduledDateTime { get; set; }
        
        /// <summary>
        /// The case number
        /// </summary>
        public string CaseNumber { get; set; }
        
        /// <summary>
        /// The case name
        /// </summary>
        public string CaseName { get; set; }

        /// <summary>
        /// The current conference status
        /// </summary>
        public ConferenceState Status { get; set; }

        /// <summary>
        /// The conference closed datetime
        /// </summary>
        public DateTime? ClosedDateTime { get; set; }

        /// <summary>
        /// Flags true when hearing venue is in Scotland
        /// </summary>
        public bool HearingVenueIsScottish { get; set; }
    }
}
