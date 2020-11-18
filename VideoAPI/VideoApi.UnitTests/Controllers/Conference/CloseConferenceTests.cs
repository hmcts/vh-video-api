using System;
using Moq;
using NUnit.Framework;
using VideoApi.Contract.Responses;
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
            var meetingRoom = new MeetingRoom("admin", "judge", "participant", "node", 12345678);
            VideoPlatformServiceMock.Setup(v => v.GetVirtualCourtRoomAsync(It.IsAny<Guid>())).ReturnsAsync(meetingRoom);
            await Controller.CloseConferenceAsync(Guid.NewGuid());

            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<CloseConferenceCommand>()), Times.Once);
            VideoPlatformServiceMock.Verify(v => v.DeleteVirtualCourtRoomAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Test]
        public async Task Should_close_conference_and_not_call_delete_audio_recording_application_if_audio_required_set_to_false_for_given_conference()
        {
            var response = new AudioPlatformServiceResponse(true);
            AudioPlatformServiceMock.Setup(v => v.DeleteAudioApplicationAsync(It.IsAny<Guid>())).ReturnsAsync(response);
            await Controller.CloseConferenceAsync(Guid.NewGuid());

            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<CloseConferenceCommand>()), Times.Once);
            AudioPlatformServiceMock.Verify(v => v.DeleteAudioApplicationAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Test]
        public async Task Should_close_conference_and_delete_audio_recording_application_if_audio_required_set_to_true_for_given_conference()
        {
            TestConference.AudioRecordingRequired = true;
            QueryHandlerMock
              .Setup(x => x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()))
              .ReturnsAsync(TestConference);
            var response = new AudioPlatformServiceResponse(true);
            AudioPlatformServiceMock.Setup(v => v.DeleteAudioApplicationAsync(It.IsAny<Guid>())).ReturnsAsync(response);
            AzureStorageServiceFactoryMock.Setup(x => x.Create(AzureStorageServiceType.Vh)).Returns(AzureStorageServiceMock.Object);

            AzureStorageServiceMock.Setup(x => x.FileExistsAsync(It.IsAny<string>())).ReturnsAsync(true);


            await Controller.CloseConferenceAsync(Guid.NewGuid());

            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<CloseConferenceCommand>()), Times.Once);
            AudioPlatformServiceMock.Verify(v => v.DeleteAudioApplicationAsync(It.IsAny<Guid>()), Times.Once);
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

            AzureStorageServiceMock.Setup(x => x.FileExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

            AudioPlatformServiceMock.Reset();
            AzureStorageServiceMock.Reset();

            await Controller.CloseConferenceAsync(Guid.NewGuid());

            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<CloseConferenceCommand>()), Times.Once);
            AzureStorageServiceMock.Verify(x => x.FileExistsAsync(It.IsAny<string>()), Times.Once);

            AudioPlatformServiceMock.Verify(v => v.DeleteAudioApplicationAsync(It.IsAny<Guid>()), Times.Never);
        }

    }
}
