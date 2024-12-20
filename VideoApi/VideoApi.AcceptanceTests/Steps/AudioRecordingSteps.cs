using System;
using System.Net;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Testing.Common;
using VideoApi.AcceptanceTests.Contexts;
using VideoApi.Contract.Responses;
using static Testing.Common.Helper.ApiUriFactory.AudioRecordingEndpoints;
using AzureStorageManager = Testing.Common.AcCommon.AzureStorageManager;

namespace VideoApi.AcceptanceTests.Steps
{
    [Binding]
    public class AudioRecordingSteps
    {
        private readonly ConferenceSteps _conferenceSteps;
        private readonly TestContext _context;
        
        public AudioRecordingSteps(TestContext context, ConferenceSteps conferenceSteps)
        {
            _context = context;
            _conferenceSteps = conferenceSteps;
        }
        
        [Given(@"the conference has an audio application")]
        public void GivenTheConferenceHasAnAudioApplication()
        {
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        
        [Given(@"I have a valid get audio stream request")]
        public void GivenIHaveAValidGetAudioStreamRequest()
        {
            _context.Request = TestContext.Get(GetAudioStream(_context.Test.ConferenceResponse.HearingId));
        }
        
        [Given(@"I have a valid get audio stream request that has no stream")]
        public void GivenIHaveAValidGetAudioStreamRequestThatHasNoStream()
        {
            _context.Request = TestContext.Get(GetAudioStream(Guid.NewGuid()));
        }
        
        [Given(@"the conference has an audio recording")]
        public async Task GivenTheConferenceHasAnAudioRecording()
        {
            var hearingId = _context.Test.ConferenceResponse.HearingId;
            var file = FileManager.CreateNewAudioFile("TestAudioFile.mp4", hearingId.ToString());
            
            _context.AzureStorage = new AzureStorageManager()
                .SetStorageAccountName(_context.Config.Wowza.StorageAccountName)
                .SetStorageAccountKey(_context.Config.Wowza.StorageAccountKey)
                .SetStorageContainerName(_context.Config.Wowza.StorageContainerName)
                .CreateBlobClient(hearingId.ToString());
            
            await _context.AzureStorage.UploadAudioFileToStorage(file);
            FileManager.RemoveLocalAudioFile(file);
        }
        
        [Given(@"I have a valid get audio recording link request")]
        public void GivenIHaveAValidGetAudioRecordingLinkRequest()
        {
            _context.Request = TestContext.Get(GetAudioRecordingLink(_context.Test.ConferenceResponse.HearingId));
        }
        
        [Given(@"I have a valid get audio recording link request for non existing hearing")]
        public void GivenIHaveAValidGetAudioRecordingLinkRequestForNonExistingHearing()
        {
            _context.Request = TestContext.Get(GetAudioRecordingLink(_context.Config.AudioRecordingTestIds.NonExistent));
        }
        
        [Given(@"I have a conference with an audio application and audio recording file")]
        public async Task GivenIHaveAConferenceWithAnAudioApplicationAndAudioRecordingFile()
        {
            _conferenceSteps.GivenIHaveAConference();
            await GivenTheConferenceHasAnAudioRecording();
        }
        
        [Then(@"the audio application details are retrieved")]
        public void ThenTheAudioApplicationDetailsAreRetrieved()
        {
            var audioApplication =
                ApiRequestHelper.Deserialise<AudioApplicationInfoResponse>(_context.Response.Content);
            audioApplication.Should().NotBeNull();
        }
        
        [Then(@"the audio stream details are retrieved")]
        public void ThenTheAudioStreamDetailsAreRetrieved()
        {
            var audioStream = ApiRequestHelper.Deserialise<AudioStreamInfoResponse>(_context.Response.Content);
            audioStream.Should().NotBeNull();
        }
        
        [Then(@"the audio stream monitoring details are retrieved")]
        public void ThenTheAudioStreamMonitoringDetailsAreRetrieved()
        {
            var audioMonitoring = ApiRequestHelper.Deserialise<AudioStreamMonitoringInfo>(_context.Response.Content);
            audioMonitoring.Should().NotBeNull();
        }
        
        [Then(@"the audio recording link is retrieved")]
        public void ThenTheAudioRecordingLinkIsRetrieved()
        {
            var audioLink = ApiRequestHelper.Deserialise<AudioRecordingResponse>(_context.Response.Content);
            audioLink.Should().NotBeNull();
            audioLink.AudioFileLinks.Should().NotBeNullOrEmpty();
        }
        
        [Then(@"the audio recording links are empty")]
        public void ThenTheAudioRecordingLinksAreEmpty()
        {
            var audioLink = ApiRequestHelper.Deserialise<AudioRecordingResponse>(_context.Response.Content);
            audioLink.Should().NotBeNull();
            audioLink.AudioFileLinks.Should().BeNullOrEmpty();
        }
    }
}
