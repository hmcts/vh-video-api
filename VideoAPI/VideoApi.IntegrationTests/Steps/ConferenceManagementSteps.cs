using System.Linq;
using System.Net.Http;
using System.Text;
using AcceptanceTests.Common.Api.Helpers;
using TechTalk.SpecFlow;
using Testing.Common.Helper;
using VideoApi.Contract.Requests;
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
            var request = new StartHearingRequest {Layout = HearingLayout.OnePlus7};
            var jsonBody = RequestHelper.Serialise(request);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
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
        
        [Given(@"I have a transfer participant into a hearing request")]
        public void GivenIHaveATransferParticipantIntoHearingRequest()
        {
            SetupTransferRequest(TransferType.Call);
        }
        
        [Given(@"I have a transfer participant out of a hearing request")]
        public void GivenIHaveATransferParticipantOutOfAHearingRequest()
        {
            SetupTransferRequest(TransferType.Dismiss);
        }

        private void SetupTransferRequest(TransferType transferType)
        {
            var conference = _context.Test.Conference;
            var conferenceId = conference.Id;
            _context.Uri = ApiUriFactory.ConferenceManagementEndpoints.TransferParticipant(conferenceId);
            _context.HttpMethod = HttpMethod.Post;
            var request = new TransferParticipantRequest
                {ParticipantId = conference.Participants.First().Id, TransferType = transferType};
            var jsonBody = RequestHelper.Serialise(request);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }
    }
}
