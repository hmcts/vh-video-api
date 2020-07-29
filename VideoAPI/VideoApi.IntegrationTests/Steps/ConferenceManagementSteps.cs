using System.Net.Http;
using TechTalk.SpecFlow;
using Testing.Common.Helper;
using VideoApi.IntegrationTests.Contexts;

namespace VideoApi.IntegrationTests.Steps
{
    [Binding]
    public class ConferenceManagementSteps: BaseSteps
    {
        private readonly TestContext _context;
        private readonly CommonSteps _commonSteps;
        
        public ConferenceManagementSteps(TestContext context, CommonSteps commonSteps)
        {
            _context = context;
            _commonSteps = commonSteps;
        }
        
        [Given(@"I have a start video hearing request")]
        public void GivenIHaveAStartVideoHearingRequest()
        {
            var conferenceId = _context.Test.Conference.Id;
            _context.Uri = ApiUriFactory.ConferenceManagementEndpoints.StartVideoHearing(conferenceId);
            _context.HttpMethod = HttpMethod.Post;
        }
    }
}
