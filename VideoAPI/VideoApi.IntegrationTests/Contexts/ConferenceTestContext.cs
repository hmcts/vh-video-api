using System;
using System.Collections.Generic;
using VideoApi.Contract.Responses;
using VideoApi.Domain;

namespace VideoApi.IntegrationTests.Contexts
{
    public class ConferenceTestContext
    {
        public ConferenceTestContext()
        {
            SeededConferences = new List<Guid>();
        }
        
        public ConferenceDetailsResponse ConferenceDetails { get; set; }
        public Conference SeededConference { get; set; }
        public List<Guid> SeededConferences { get; }
    }
}