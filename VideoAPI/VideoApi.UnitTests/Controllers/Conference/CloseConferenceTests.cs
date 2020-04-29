using System;
using Moq;
using NUnit.Framework;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
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
            var meetingRoom = new MeetingRoom("admin", "judge", "participant", "node");
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
            await Controller.CloseConferenceAsync(Guid.NewGuid());

            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<CloseConferenceCommand>()), Times.Once);
            AudioPlatformServiceMock.Verify(v => v.DeleteAudioApplicationAsync(It.IsAny<Guid>()), Times.Once);
        }
    }
}
