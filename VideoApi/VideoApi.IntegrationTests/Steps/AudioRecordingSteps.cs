using System;
using System.Collections.Generic;
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

        public AudioRecordingSteps(TestContext context, CommonSteps commonSteps)
        {
            _context = context;
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

        [Given(@"I have a valid get audio recording link request")]
        public void GivenIHaveAValidGetAudioRecordingLinkRequest()
        {
            _context.Uri = GetAudioRecordingLink(_context.Test.Conference.HearingRefId);
            _context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a valid get cvp audio recordings by all request for (.*) (.*) (.*)")]
        public void GivenIHaveAValidGetCvpAudioRecordingByAllRequest(string cloudRoom, string date, string caseReference)
        {
            _context.Uri = GetCvpAudioRecordingsAll(cloudRoom, date, caseReference);
            _context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a valid get cvp audio recordings by cloud room request for (.*) (.*)")]
        public void GivenIHaveAValidGetCvpAudioRecordingByCloudRoomRequest(string cloudRoom, string date)
        {
            _context.Uri = GetCvpAudioRecordingsByCloudRoom(cloudRoom, date);
            _context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a valid get cvp audio recordings by date request for (.*) (.*)")]
        public void GivenIHaveAValidGetCvpAudioRecordingByDateRequest(string date, string caseReference)
        {
            _context.Uri = GetCvpAudioRecordingsByDate(date, caseReference);
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
            audioRecording.AudioFileLinks.Should().NotBeNullOrEmpty();
        }

        [Then(@"the audio recording link details are empty")]
        public async Task ThenTheAudioRecordingLinkDetailsAreEmpty()
        {
            var audioRecording = await Response.GetResponses<AudioRecordingResponse>(_context.Response.Content);
            audioRecording.Should().NotBeNull();
            audioRecording.AudioFileLinks.Should().BeNullOrEmpty();
        }

        [Then(@"(.*) audio recordings from cvp are retrieved")]
        public async Task ThenTheCountAudioRecordingFromCvpAreRetrieved(int count)
        {
            var audioRecordings = await Response.GetResponses<List<CvpAudioFileResponse>>(_context.Response.Content);
            var countAudioRecords = audioRecordings.Count;
            countAudioRecords.Should().BeGreaterOrEqualTo(count);
        }
    }
}
