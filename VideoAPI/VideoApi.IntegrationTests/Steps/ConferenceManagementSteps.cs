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
        
        public ConferenceManagementSteps(TestContext context, CommonSteps commonSteps)
        {
            _context = context;
        }
        
        [Given(@"I have a start video hearing request")]
        public void GivenIHaveAStartVideoHearingRequest()
        {
            var conferenceId = _context.Test.Conference.Id;
            _context.Uri = ApiUriFactory.ConferenceManagementEndpoints.StartVideoHearing(conferenceId);
            _context.HttpMethod = HttpMethod.Post;
        }
        
        [Given(@"I have a pause video hearing request")]
        public void GivenIHaveAPauseVideoHearingRequest()
        {
            var conferenceId = _context.Test.Conference.Id;
            _context.Uri = ApiUriFactory.ConferenceManagementEndpoints.PauseVideoHearing(conferenceId);
            _context.HttpMethod = HttpMethod.Post;
        }
        
        [Given(@"I have a end video hearing request")]
        public void GivenIHaveAEndVideoHearingRequest()
        {
            var conferenceId = _context.Test.Conference.Id;
            _context.Uri = ApiUriFactory.ConferenceManagementEndpoints.EndVideoHearing(conferenceId);
            _context.HttpMethod = HttpMethod.Post;
        }
        
        [Given(@"I have a technical assistance request")]
        public void GivenIHaveATechnicalAssistanceRequest()
        {
            var conferenceId = _context.Test.Conference.Id;
            _context.Uri = ApiUriFactory.ConferenceManagementEndpoints.RequestTechnicalAssistance(conferenceId);
            _context.HttpMethod = HttpMethod.Post;
        }
    }
}
