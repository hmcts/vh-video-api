using System;
using System.Net;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Testing.Common;
using Testing.Common.Helper;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Common.Configuration;
using VideoApi.Contract.Responses;
using VideoApi.Domain;
using VideoApi.IntegrationTests.Api.Setup;
using VideoApi.IntegrationTests.Helper;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Api.AudioRecordings;

[Category("azurite")]
public class GetAudioRecordingLinkTests : ApiTest
{
    private Conference _conference;
    private AzureStorageManager _azureStorageManager;

    [TearDown]
    public async Task TearDown()
    {
        if (_conference != null)
        {
            await TestDataManager.RemoveConference(_conference.Id);
            _conference = null;
        }

        if (_azureStorageManager != null)
        {
            await _azureStorageManager.RemoveAudioFileFromStorage();
            _azureStorageManager = null;
        }
    }

    [Test]
    public async Task should_return_an_empty_list_when_no_recordings_are_found()
    {
        // arrange
        var hearingId = Guid.NewGuid();

        // act
        using var client = Application.CreateClient();
        var result =
            await client.GetAsync(ApiUriFactory.AudioRecordingEndpoints.GetAudioRecordingLink(hearingId));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var audioRecording = await ApiClientResponse.GetResponses<AudioRecordingResponse>(result.Content);
        audioRecording.Should().NotBeNull();
        audioRecording.AudioFileLinks.Should().BeNullOrEmpty();
    }
    
    [Test]
    public async Task should_return_a_list_of_audio_recording_for_a_hearing()
    {
        // arrange
        var conferenceWithMessages = new ConferenceBuilder(ignoreId: true).Build();
        _conference = await TestDataManager.SeedConference(conferenceWithMessages);
        await SeedAudioRecordingIntoStorage(_conference.HearingRefId);
        
        // act
        using var client = Application.CreateClient();
        var result =
            await client.GetAsync(ApiUriFactory.AudioRecordingEndpoints.GetAudioRecordingLink(_conference.HearingRefId));


        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var audioRecording = await ApiClientResponse.GetResponses<AudioRecordingResponse>(result.Content);
        audioRecording.Should().NotBeNull();
        audioRecording.AudioFileLinks.Should().NotBeNullOrEmpty();

    }

    private async Task SeedAudioRecordingIntoStorage(Guid hearingRefId)
    {
        var wowzaConfiguration = ConfigRoot.GetSection("WowzaConfiguration").Get<WowzaConfiguration>();
        var azureStorageConnectionString = ConfigRoot.GetValue<string>("Azure:StorageConnectionString");
        _azureStorageManager = new AzureStorageManager()
            .SetStorageContainerName(wowzaConfiguration.StorageContainerName)
            .CreateBlobClient(hearingRefId.ToString(), azureStorageConnectionString);

        var file = FileManager.CreateNewAudioFile("TestAudioFile.mp4", hearingRefId.ToString());

        await _azureStorageManager.UploadAudioFileToStorage(file);
        FileManager.RemoveLocalAudioFile(file);
    }
}
