using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common;
using Testing.Common.Helper.Builders.Api;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;

namespace VideoApi.AcceptanceTests.ApiTests.AudioRecordings;

public class GetAudioRecordingLinkTests : AcApiTest
{
    private ConferenceDetailsResponse _conference;
    private AzureStorageManager _azureStorageManager;

    [TearDown]
    public void TearDown()
    {
        
    }

    [Test]
    public async Task should_return_okay_and_audio_recording_link_when_hearing_does_exist()
    {
        // arrange
        var request = CreateNewConferenceRequest(DateTime.UtcNow.AddMinutes(2), true);
        _conference = await VideoApiClient.BookNewConferenceAsync(request);

        await SeedAudioRecording(_conference.HearingId);
        
        // act 
        var links = await VideoApiClient.GetAudioRecordingLinkAsync(_conference.HearingId);

        // assert
        links.Should().NotBeNull();
        links.AudioFileLinks.Should().NotBeNullOrEmpty();
    }
    
    [Test]
    public async Task should_return_okay_and_an_empty_list_when_hearing_does_not_exist()
    {
        // arrange
        var request = CreateNewConferenceRequest(DateTime.UtcNow.AddMinutes(2), true);
        _conference = await VideoApiClient.BookNewConferenceAsync(request);
        
        // act 
        var links = await VideoApiClient.GetAudioRecordingLinkAsync(_conference.HearingId);

        // assert
        links.Should().NotBeNull();
        links.AudioFileLinks.Should().BeEmpty();
    }
    
    private BookNewConferenceRequest CreateNewConferenceRequest(DateTime date, bool audioRequired = false)
    {
        var request = new BookNewConferenceRequestBuilder("AC Test Return Stream Info")
            .WithJudge()
            .WithIndividual("Respondent")
            .WithHearingRefId(Guid.NewGuid())
            .WithDate(date)
            .WithAudiorecordingRequired(audioRequired)
            .Build();
        return request;
    }

    private async Task SeedAudioRecording(Guid hearingId)
    {
        var file = FileManager.CreateNewAudioFile("TestAudioFile.mp4", hearingId.ToString());

        _azureStorageManager = new AzureStorageManager(WowzaConfiguration.StorageAccountName,
            WowzaConfiguration.StorageAccountKey, WowzaConfiguration.StorageContainerName)
            .CreateBlobClient(file);
        
        await _azureStorageManager.UploadAudioFileToStorage(file);
        FileManager.RemoveLocalAudioFile(file);
    }
}
