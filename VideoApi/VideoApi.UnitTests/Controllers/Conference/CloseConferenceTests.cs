using System;
using System.Collections.Generic;
using Moq;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using VideoApi.Services.Contracts;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Conference
{
    public class CloseConferenceTests : ConferenceControllerTestBase
    {
        [Test]
        public async Task Should_close_conference_for_given_valid_conference_id()
        {
            VideoPlatformServiceMock.Setup(v => v.GetVirtualCourtRoomAsync(It.IsAny<Guid>())).ReturnsAsync((MeetingRoom)null);

            await Controller.CloseConferenceAsync(Guid.NewGuid());

            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<CloseConferenceCommand>()), Times.Once);
            VideoPlatformServiceMock.Verify(v => v.DeleteVirtualCourtRoomAsync(It.IsAny<Guid>()), Times.Never);
        }
        
        [Test]
        public async Task Should_close_conference_and_remove_court_room_for_given_valid_conference_id()
        {
            var meetingRoom = new MeetingRoom("admin", "judge", "participant", "node", "12345678");
            VideoPlatformServiceMock.Setup(v => v.GetVirtualCourtRoomAsync(It.IsAny<Guid>())).ReturnsAsync(meetingRoom);
            await Controller.CloseConferenceAsync(Guid.NewGuid());

            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<CloseConferenceCommand>()), Times.Once);
            VideoPlatformServiceMock.Verify(v => v.DeleteVirtualCourtRoomAsync(It.IsAny<Guid>()), Times.Once);
        }
        
        [Test]
        public async Task Should_close_conference_and_not_call_delete_audio_recording_application_if_audio_required_set_to_false_for_given_conference()
        {
            await Controller.CloseConferenceAsync(Guid.NewGuid());
            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<CloseConferenceCommand>()), Times.Once);
        }
        
        [Test]
        public async Task Should_close_conference_and_delete_audio_recording_application_if_audio_required_set_to_true_for_given_conference()
        {
            TestConference.AudioRecordingRequired = true;
            QueryHandlerMock
              .Setup(x => x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()))
              .ReturnsAsync(TestConference);
            AzureStorageServiceFactoryMock.Setup(x => x.Create(AzureStorageServiceType.Vh)).Returns(AzureStorageServiceMock.Object);

            var filesNames = new List<string> { "SomeBlob.mp4" };
            AzureStorageServiceMock.Setup(x => x.GetAllBlobNamesByFilePathPrefix(It.IsAny<string>()))
                .ReturnsAsync(filesNames);

            await Controller.CloseConferenceAsync(Guid.NewGuid());

            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<CloseConferenceCommand>()), Times.Once);
        }
        
        [Test]
        public async Task Should_close_conference_and_delete_audio_recording_application_if_audio_files_exist_and_actual_start_date_is_null()
        {
            TestConference.AudioRecordingRequired = true;
            QueryHandlerMock
              .Setup(x => x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()))
              .ReturnsAsync(TestConference);
            AzureStorageServiceFactoryMock.Setup(x => x.Create(AzureStorageServiceType.Vh)).Returns(AzureStorageServiceMock.Object);

            AzureStorageServiceMock.Setup(x => x.GetAllBlobNamesByFilePathPrefix(It.IsAny<string>()))
                .ReturnsAsync(new List<string> { $"{TestConference.HearingRefId.ToString()}.mp4" });

            await Controller.CloseConferenceAsync(Guid.NewGuid());

            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<CloseConferenceCommand>()), Times.Once);
        }
        
        [Test]
        public async Task Should_close_conference_and_not_call_delete_audio_recording_application_if_audio_recording_file_not_found()
        {
            TestConference.AudioRecordingRequired = true;
            TestConference.UpdateConferenceStatus(VideoApi.Domain.Enums.ConferenceState.InSession);

            QueryHandlerMock
               .Setup(x => x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()))
               .ReturnsAsync(TestConference);
            AzureStorageServiceFactoryMock.Setup(x => x.Create(AzureStorageServiceType.Vh)).Returns(AzureStorageServiceMock.Object);
            AudioPlatformServiceMock.Reset();
            AzureStorageServiceMock.Reset();

            AzureStorageServiceMock.Setup(x => x.GetAllBlobNamesByFilePathPrefix(It.IsAny<string>())).ReturnsAsync(new List<string>());

            await Controller.CloseConferenceAsync(Guid.NewGuid());

            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<CloseConferenceCommand>()), Times.Once);
        }
        
        [Test]
        public async Task Should_close_conference_and_not_call_delete_audio_recording_application_if_audio_recording_is_on_single_instance()
        {
            TestConference.AudioRecordingRequired = true;
            TestConference.IngestUrl = $"rtmps://vh-wowza-dev.hearings.reform.hmcts.net:443/{AppName}/0dc59e0c-a5f8-4b05-868f-00e96c23ca79";
            TestConference.UpdateConferenceStatus(VideoApi.Domain.Enums.ConferenceState.InSession);

            QueryHandlerMock
                .Setup(x => x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync(TestConference);
            AzureStorageServiceFactoryMock.Setup(x => x.Create(AzureStorageServiceType.Vh)).Returns(AzureStorageServiceMock.Object);
            AudioPlatformServiceMock.Reset();
            AzureStorageServiceMock.Reset();

            AzureStorageServiceMock.Setup(x => x.GetAllBlobNamesByFilePathPrefix(It.IsAny<string>())).ReturnsAsync(new List<string>());

            await Controller.CloseConferenceAsync(Guid.NewGuid());

            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<CloseConferenceCommand>()), Times.Once);
        }
    }
}
