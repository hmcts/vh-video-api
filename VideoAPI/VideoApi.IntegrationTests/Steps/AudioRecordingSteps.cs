using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AcceptanceTests.Common.AudioRecordings;
using FluentAssertions;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Testing.Common.Configuration;
using VideoApi.Contract.Responses;
using VideoApi.IntegrationTests.Contexts;
using static Testing.Common.Helper.ApiUriFactory.AudioRecordingEndpoints;
using Response = VideoApi.IntegrationTests.Helper.Response;

namespace VideoApi.IntegrationTests.Steps
{
    [Binding]
    public sealed class AudioRecordingSteps : BaseSteps
    {
        private readonly TestContext _context;
        private readonly CommonSteps _commonSteps;

        public AudioRecordingSteps(TestContext context, CommonSteps commonSteps)
        {
            _context = context;
            _commonSteps = commonSteps;
        }

        [Given(@"the conference has an audio application")]
        public async Task GivenTheConferenceHasAnAudioApplication()
        {
            GivenIHaveAValidCreateAudioApplicationRequest();
            await _commonSteps.WhenISendTheRequestToTheEndpoint();
            _context.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Given(@"the conference has an application and an audio stream")]
        public async Task GivenTheConferenceHasAnApplicationAndAnAudioStream()
        {
            GivenIHaveAValidCreateAudioApplicationAndStreamRequest();
            await _commonSteps.WhenISendTheRequestToTheEndpoint();
            _context.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Given(@"the conference has an audio stream")]
        public async Task GivenTheConferenceHasAnAudioStream()
        {
            GivenIHaveAValidCreateAudioStreamRequest();
            await _commonSteps.WhenISendTheRequestToTheEndpoint();
            _context.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Given(@"I have an audio recording")]
        public async Task GivenIHaveAnAudioRecording()
        {
            var file = FileManager.CreateNewAudioFile("TestAudioFile.mp4", _context.Test.Conference.HearingRefId.ToString());

            _context.AzureStorage = new AzureStorageManager()
                .SetStorageAccountName(_context.Config.Wowza.StorageAccountName)
                .SetStorageAccountKey(_context.Config.Wowza.StorageAccountKey)
                .SetStorageContainerName(_context.Config.Wowza.StorageContainerName)
                .CreateBlobClient(_context.Test.Conference.HearingRefId.ToString());

            await _context.AzureStorage.UploadAudioFileToStorage(file);
            FileManager.RemoveLocalAudioFile(file);
        }

        [Given(@"Cvp has audio recordings")]
        public async Task CvpHasAudioRecordings(Table table)
        {
            var parameters = table.CreateSet<CvpGetAudioFileParameters>();
            var file = FileManager.CreateNewAudioFile("TestAudioFile.mp4", Guid.NewGuid().ToString());

            _context.AzureStorage = new AzureStorageManager()
                .SetStorageAccountName(_context.Config.Cvp.StorageAccountName)
                .SetStorageAccountKey(_context.Config.Cvp.StorageAccountKey)
                .SetStorageContainerName(_context.Config.Cvp.StorageContainerName)
                .CreateBlobContainerClient();

            _context.Test.CvpFileNamesOnStorage = new List<string>();
            
            foreach (var cvp in parameters)
            {
                var filePathOnStorage = $"audiostream{cvp.CloudRoom}/{cvp.CaseReference}-{cvp.Date}-{Guid.NewGuid()}.mp4";
                _context.Test.CvpFileNamesOnStorage.Add(filePathOnStorage);

                await _context.AzureStorage.UploadFileToStorage(file, filePathOnStorage);
            }
            
            FileManager.RemoveLocalAudioFile(file);
        }

        [Given(@"I have a valid get audio application request")]
        public void GivenIHaveAValidGetAudioApplicationRequest()
        {
            _context.Uri = GetAudioApplication(_context.Test.Conference.HearingRefId);
            _context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a nonexistent get audio application request")]
        public void GivenIHaveANonexistentGetAudioApplicationRequest()
        {
            _context.Uri = GetAudioApplication(Guid.NewGuid());
            _context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a valid create audio application request")]
        [When(@"I have a duplicate create audio application request")]
        public void GivenIHaveAValidCreateAudioApplicationRequest()
        {
            _context.Uri = CreateAudioApplication(_context.Test.Conference.HearingRefId);
            _context.HttpMethod = HttpMethod.Post;
        }

        [Given(@"I have a nonexistent create audio application request")]
        public void GivenIHaveANonexistentCreateAudioApplicationRequest()
        {
            _context.Uri = CreateAudioApplication(Guid.NewGuid());
            _context.HttpMethod = HttpMethod.Post;
        }

        [Given(@"I have a valid delete audio application request")]
        public void GivenIHaveAValidDeleteAudioApplicationRequest()
        {
            _context.Uri = DeleteAudioApplication(_context.Test.Conference.HearingRefId);
            _context.HttpMethod = HttpMethod.Delete;
        }

        [Given(@"I have a nonexistent delete audio application request")]
        public void GivenIHaveANonexistentDeleteAudioApplicationRequest()
        {
            _context.Uri = DeleteAudioApplication(Guid.NewGuid());
            _context.HttpMethod = HttpMethod.Delete;
        }

        [Given(@"I have a valid create audio application and stream request")]
        [When(@"I have a duplicate create audio application and stream request")]
        public void GivenIHaveAValidCreateAudioApplicationAndStreamRequest()
        {
            _context.Uri = CreateAudioApplicationAndStream(_context.Test.Conference.HearingRefId);
            _context.HttpMethod = HttpMethod.Post;
        }

        [Given(@"I have a nonexistent create audio application and stream request")]
        public void GivenIHaveANonexistentCreateAudioApplicationAndStreamRequest()
        {
            _context.Uri = CreateAudioApplicationAndStream(Guid.NewGuid());
            _context.HttpMethod = HttpMethod.Post;
        }

        [Given(@"I have a valid get audio stream request")]
        public void GivenIHaveAValidGetAudioStreamRequest()
        {
            _context.Uri = GetAudioStream(_context.Test.Conference.HearingRefId);
            _context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a nonexistent get audio steam request")]
        public void GivenIHaveANonexistentGetAudioSteamRequest()
        {
            _context.Uri = GetAudioStream(Guid.NewGuid());
            _context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a valid create audio stream request")]
        [When(@"I have a duplicate create audio stream request")]
        public void GivenIHaveAValidCreateAudioStreamRequest()
        {
            _context.Uri = CreateAudioStream(_context.Test.Conference.HearingRefId);
            _context.HttpMethod = HttpMethod.Post;
        }

        [Given(@"I have a nonexistent create audio stream request")]
        public void GivenIHaveANonexistentCreateAudioStreamRequest()
        {
            _context.Uri = CreateAudioStream(Guid.NewGuid());
            _context.HttpMethod = HttpMethod.Post;
        }

        [Given(@"I have a valid delete audio stream request")]
        public void GivenIHaveAValidDeleteAudioStreamRequest()
        {
            _context.Uri = DeleteAudioStream(_context.Test.Conference.HearingRefId);
            _context.HttpMethod = HttpMethod.Delete;
        }

        [Given(@"I have a nonexistent delete audio stream request")]
        public void GivenIHaveANonexistentDeleteAudioStreamRequest()
        {
            _context.Uri = DeleteAudioStream(Guid.NewGuid());
            _context.HttpMethod = HttpMethod.Delete;
        }

        [Given(@"I have a valid get audio monitoring stream request")]
        public void GivenIHaveAValidGetAudioMonitoringStreamRequest()
        {
            _context.Uri = GetAudioMonitoringStream(_context.Test.Conference.HearingRefId);
            _context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a nonexistent get audio monitoring steam request")]
        public void GivenIHaveANonexistentGetAudioMonitoringSteamRequest()
        {
            _context.Uri = GetAudioMonitoringStream(Guid.NewGuid());
            _context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a valid get audio recording link request")]
        public void GivenIHaveAValidGetAudioRecordingLinkRequest()
        {
            _context.Uri = GetAudioRecordingLink(_context.Test.Conference.HearingRefId);
            _context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a valid get cvp audio recordings request for (.*) (.*) (.*)")]
        public void GivenIHaveAValidGetCvpAudioRecordingRequest(string cloudRoom, string date, string caseReference)
        {
            _context.Uri = !string.IsNullOrWhiteSpace(caseReference) 
                ? GetCvpAudioRecordings(cloudRoom, date, caseReference)
                : GetCvpAudioRecordings(cloudRoom, date);
            _context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a nonexistent get audio recording link request")]
        public void GivenIHaveANonexistentGetAudioRecordingLinkRequest()
        {
            _context.Uri = GetAudioRecordingLink(Guid.NewGuid());
            _context.HttpMethod = HttpMethod.Get;
        }

        [Then(@"the audio application details are retrieved")]
        public async Task ThenTheAudioApplicationDetailsAreRetrieved()
        {
            var audioApplication = await Response.GetResponses<AudioApplicationInfoResponse>(_context.Response.Content);
            audioApplication.Should().NotBeNull();
        }

        [Then(@"the audio stream details are retrieved")]
        public async Task ThenTheAudioStreamDetailsAreRetrieved()
        {
            var audioStream = await Response.GetResponses<AudioStreamInfoResponse>(_context.Response.Content);
            audioStream.Should().NotBeNull();
        }

        [Then(@"the audio monitoring stream details are retrieved")]
        public async Task ThenTheAudioMonitoringStreamDetailsAreRetrieved()
        {
            var audioMonitoringStream = await Response.GetResponses<AudioStreamMonitoringInfo>(_context.Response.Content);
            audioMonitoringStream.Should().NotBeNull();
        }

        [Then(@"the audio recording link details are retrieved")]
        public async Task ThenTheAudioRecordingLinkDetailsAreRetrieved()
        {
            var audioRecording = await Response.GetResponses<AudioRecordingResponse>(_context.Response.Content);
            audioRecording.Should().NotBeNull();
        }

        [Then(@"(.*) audio recordings from cvp are retrieved")]
        public async Task ThenTheCountAudioRecordingFromCvpAreRetrieved(int count)
        {
            var audioRecordings = await Response.GetResponses<List<CvpAudioFileResponse>>(_context.Response.Content);
            audioRecordings.Should().HaveCount(count);
        }
    }
}
