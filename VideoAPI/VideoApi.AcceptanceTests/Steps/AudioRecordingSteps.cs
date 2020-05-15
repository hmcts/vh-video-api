using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using AcceptanceTests.Common.AudioRecordings;
using FluentAssertions;
using TechTalk.SpecFlow;
using VideoApi.AcceptanceTests.Contexts;
using VideoApi.Contract.Responses;
using static Testing.Common.Helper.ApiUriFactory.AudioRecordingEndpoints;

namespace VideoApi.AcceptanceTests.Steps
{
    [Binding]
    public class AudioRecordingSteps
    {
        private readonly TestContext _context;

        public AudioRecordingSteps(TestContext context)
        {
            _context = context;
        }

        [Given(@"I have a valid create audio application request")]
        public void GivenIHaveACreateAudioApplicationRequest()
        {
            _context.Request = _context.Post(CreateAudioApplication(_context.Test.ConferenceResponse.HearingId));
        }

        [Given(@"I have a valid get audio application request")]
        public void GivenIHaveAValidGetAudioApplicationRequest()
        {
            _context.Request = _context.Get(GetAudioApplication(_context.Test.ConferenceResponse.HearingId));
        }

        [Given(@"I have a valid delete audio application request")]
        public void GivenIHaveAValidDeleteAudioApplicationRequest()
        {
            _context.Request = _context.Delete(DeleteAudioApplication(_context.Test.ConferenceResponse.HearingId));
        }

        [Given(@"the conference has an audio application")]
        public void GivenTheConferenceHasAnAudioApplication()
        {
            GivenIHaveACreateAudioApplicationRequest();
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Given(@"I have a valid create audio application and stream request")]
        public void GivenIHaveAValidCreateAudioApplicationAndStreamRequest()
        {
            _context.Request = _context.Post(CreateAudioApplicationAndStream(_context.Test.ConferenceResponse.HearingId));
        }

        [Given(@"I have a valid get audio stream request")]
        public void GivenIHaveAValidGetAudioStreamRequest()
        {
            _context.Request = _context.Get(GetAudioStream(_context.Test.ConferenceResponse.HearingId));
        }

        [Given(@"I have a valid create audio stream request")]
        public void GivenIHaveAValidCreateAudioStreamRequest()
        {
            _context.Request = _context.Post(CreateAudioStream(_context.Test.ConferenceResponse.HearingId));
        }

        [Given(@"I have a valid delete audio stream request")]
        public void GivenIHaveAValidDeleteAudioStreamRequest()
        {
            _context.Request = _context.Delete(DeleteAudioStream(_context.Test.ConferenceResponse.HearingId));
        }

        [Given(@"I have a valid get audio stream monitoring request")]
        public void GivenIHaveAValidGetAudioStreamMonitoringRequest()
        {
            _context.Request = _context.Get(GetAudioMonitoringStream(_context.Test.ConferenceResponse.HearingId));
        }

        [Given(@"the conference has an audio stream")]
        public void GivenTheConferenceHasAnAudioStream()
        {
            GivenTheConferenceHasAnAudioApplication();
            GivenIHaveAValidCreateAudioStreamRequest();
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Given(@"the conference has an audio recording")]
        public async Task GivenTheConferenceHasAnAudioRecording()
        {
            var file = AudioRecordingsManager.CreateNewAudioFile("TestAudioFile.mp4", _context.Test.ConferenceResponse.HearingId);
            
            var wowsaManager = new WowzaManager()
                .SetStorageAccountName(_context.Config.Wowza.StorageAccountName)
                .SetStorageAccountKey(_context.Config.Wowza.StorageAccountKey)
                .SetStorageContainerName(_context.Config.Wowza.StorageContainerName)
                .CreateBlobClient(_context.Test.ConferenceResponse.HearingId);

            await wowsaManager.UploadAudioFileToStorage(file);
        }

        [Given(@"I have a valid get audio recording link request")]
        public void GivenIHaveAValidGetAudioRecordingLinkRequest()
        {
            _context.Request = _context.Get(GetAudioRecordingLink(_context.Test.ConferenceResponse.HearingId));
        }

        [Then(@"the audio application details are retrieved")]
        public void ThenTheAudioApplicationDetailsAreRetrieved()
        {
            var audioApplication = RequestHelper.DeserialiseSnakeCaseJsonToResponse<AudioApplicationInfoResponse>(_context.Response.Content);
            audioApplication.Should().NotBeNull();
        }

        [Then(@"the audio stream details are retrieved")]
        public void ThenTheAudioStreamDetailsAreRetrieved()
        {
            var audioStream = RequestHelper.DeserialiseSnakeCaseJsonToResponse<AudioStreamInfoResponse>(_context.Response.Content);
            audioStream.Should().NotBeNull();
        }

        [Then(@"the audio stream monitoring details are retrieved")]
        public void ThenTheAudioStreamMonitoringDetailsAreRetrieved()
        {
            var audioMonitoring = RequestHelper.DeserialiseSnakeCaseJsonToResponse<AudioStreamMonitoringInfo>(_context.Response.Content);
            audioMonitoring.Should().NotBeNull();
        }

        [Then(@"the audio recording link is retrieved")]
        public void ThenTheAudioRecordingLinkIsRetrieved()
        {
            var audioLink = RequestHelper.DeserialiseSnakeCaseJsonToResponse<AudioRecordingResponse>(_context.Response.Content);
            audioLink.Should().NotBeNull();
        }
    }
}
