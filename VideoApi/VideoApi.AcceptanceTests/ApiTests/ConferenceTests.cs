using System;
using System.Threading.Tasks;
using Bogus;
using NUnit.Framework;
using Testing.Common;
using Testing.Common.Helper.Builders.Api;
using VideoApi.Contract.Responses;
using AzureStorageManager = Testing.Common.AcCommon.AzureStorageManager;

namespace VideoApi.AcceptanceTests.ApiTests;

public class ConferenceTests : AcApiTest
{
    private ConferenceDetailsResponse _conference;
    private AzureStorageManager _azureStorage;

    [Test]
    public async Task should_book_a_conference_with_jvs_endpoints_and_confirm_audio_links()
    {
        Faker faker = new();
        var date = DateTime.Now.ToLocalTime().AddMinutes(2);
        var sipStem = GetSupplierSipAddressStem();
        var request = new BookNewConferenceRequestBuilder("AC InstantMessage Tests", sipStem)
            .WithJudge()
            .WithRepresentative().WithIndividual()
            .WithHearingRefId(Guid.NewGuid())
            .WithDate(date)
            .WithEndpoint("One", $"{faker.Random.Long(1000000000, 9999999999)}", "1234")
            .WithEndpoint("Two", $"{faker.Random.Long(1000000000, 9999999999)}", "2345")
            .Build();
        _conference = await VideoApiClient.BookNewConferenceAsync(request);
        
        _conference.Should().NotBeNull();

        var getResult = await VideoApiClient.GetConferenceDetailsByIdAsync(_conference.Id);
        
        getResult.Should().NotBeNull();
        getResult.Should().BeEquivalentTo(_conference);

        await SeedAudioRecordingForConference(_conference.HearingId.ToString());

        var audioRecordingLink = await VideoApiClient.GetAudioRecordingLinkAsync(_conference.HearingId.ToString());
        audioRecordingLink.Should().NotBeNull();
        audioRecordingLink.AudioFileLinks.Should().NotBeNullOrEmpty();
    }

    private async Task SeedAudioRecordingForConference(string hearingId)
    {
        var file = FileManager.CreateNewAudioFile("TestAudioFile.mp4", hearingId);
        _azureStorage = new AzureStorageManager()
            .SetStorageAccountName(WowzaConfiguration.StorageAccountName)
            .SetStorageAccountKey(WowzaConfiguration.StorageAccountKey)
            .SetStorageContainerName(WowzaConfiguration.StorageContainerName)
            .CreateBlobClient(hearingId);

        await _azureStorage.UploadAudioFileToStorage(file);
        FileManager.RemoveLocalAudioFile(file);
    }
    
    [TearDown]
    public async Task TearDown()
    {
        if (_conference != null)
        {
            await _azureStorage.RemoveAudioFileFromStorage();
            await VideoApiClient.RemoveConferenceAsync(_conference.Id);
            _conference = null;
        }
    }
}
