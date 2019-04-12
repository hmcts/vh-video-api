using VideoApi.Contract.Responses;
using VideoApi.Domain;

namespace VideoApi.IntegrationTests.Contexts
{
    public class ConferenceTestContext
    {
        public ConferenceDetailsResponse ConferenceDetails { get; set; }
        public Conference SeededConference { get; set; }
    }
}