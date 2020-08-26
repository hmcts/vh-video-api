using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Api;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using VideoApi.Services.Dtos;
using VideoApi.Services.Exceptions;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Conference
{
    public class BookNewConferenceTests : ConferenceControllerTestBase
    {
        private BookNewConferenceRequest _request;

        [SetUp]
        public void TestInitialize()
        {
            _request = new BookNewConferenceRequestBuilder("Video Api Unit Test Hearing")
               .WithJudge()
               .WithRepresentative("Claimant").WithIndividual("Claimant")
               .WithRepresentative("Defendant").WithIndividual("Defendant")
               .WithEndpoint("DisplayName", "1234567890", "1234")
               .Build();
        }

        [Test]
        public async Task Should_book_kinly_conference_room_for_given_conference_id()
        {
            var audioPlatformServiceResponse = new AudioPlatformServiceResponse(true) {IngestUrl = "http://myIngestUrl.com"};
            AudioPlatformServiceMock.Setup(x => x.CreateAudioApplicationWithStreamAsync(_request.HearingRefId)).ReturnsAsync(audioPlatformServiceResponse);
            VideoPlatformServiceMock.Setup(v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<IEnumerable<EndpointDto>>())).ReturnsAsync((MeetingRoom)null);
            
            await Controller.BookNewConferenceAsync(_request);

            VideoPlatformServiceMock.Verify(v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), It.IsAny<bool>(), audioPlatformServiceResponse.IngestUrl, It.IsAny<IEnumerable<EndpointDto>>()), Times.Once);
            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<UpdateMeetingRoomCommand>()), Times.Never);
            QueryHandlerMock.Verify(q => q.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()), Times.Once);
        }

        [Test]
        public async Task Should_verify_double_booking_for_given_conference_id()
        {
            var audioPlatformServiceResponse = new AudioPlatformServiceResponse(true) {IngestUrl = "http://myIngestUrl.com"};
            AudioPlatformServiceMock.Setup(x => x.CreateAudioApplicationWithStreamAsync(_request.HearingRefId)).ReturnsAsync(audioPlatformServiceResponse);
            VideoPlatformServiceMock.Setup(v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<IEnumerable<EndpointDto>>())).Throws(new DoubleBookingException(Guid.NewGuid()));

            await Controller.BookNewConferenceAsync(_request);

            VideoPlatformServiceMock.Verify(v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), It.IsAny<bool>(), audioPlatformServiceResponse.IngestUrl, It.IsAny<IEnumerable<EndpointDto>>()), Times.Once);
            VideoPlatformServiceMock.Verify(v => v.GetVirtualCourtRoomAsync(It.IsAny<Guid>()), Times.Once);
            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<UpdateMeetingRoomCommand>()), Times.Never);
            QueryHandlerMock.Verify(q => q.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()), Times.Once);
        }

        [Test]
        public async Task Should_book_kinly_conference_and_update_meeting_room_for_given_conference_id()
        {
            var audioPlatformServiceResponse = new AudioPlatformServiceResponse(true) {IngestUrl = "http://myIngestUrl.com"};
            AudioPlatformServiceMock.Setup(x => x.CreateAudioApplicationWithStreamAsync(_request.HearingRefId)).ReturnsAsync(audioPlatformServiceResponse);
            VideoPlatformServiceMock.Setup(v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), It.IsAny<bool>(), audioPlatformServiceResponse.IngestUrl, It.IsAny<IEnumerable<EndpointDto>>())).ReturnsAsync(MeetingRoom);

            await Controller.BookNewConferenceAsync(_request);

            VideoPlatformServiceMock.Verify(v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), It.IsAny<bool>(), audioPlatformServiceResponse.IngestUrl, It.IsAny<IEnumerable<EndpointDto>>()), Times.Once);
            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<UpdateMeetingRoomCommand>()), Times.Once);
            QueryHandlerMock.Verify(q => q.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()), Times.Once);
        }
        
        [Test]
        public async Task Should_book_kinly_conference_with_ingesturl_when_audio_recording_not_required()
        {
            var audioPlatformServiceResponse = new AudioPlatformServiceResponse(true) {IngestUrl = "http://myIngestUrl.com"};
            AudioPlatformServiceMock.Setup(x => x.CreateAudioApplicationWithStreamAsync(_request.HearingRefId)).ReturnsAsync(audioPlatformServiceResponse);
            VideoPlatformServiceMock.Setup(v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), false, audioPlatformServiceResponse.IngestUrl, It.IsAny<IEnumerable<EndpointDto>>())).ReturnsAsync(MeetingRoom);

            _request.AudioRecordingRequired = false;
            await Controller.BookNewConferenceAsync(_request);

            AudioPlatformServiceMock.Verify(x => x.CreateAudioApplicationWithStreamAsync(It.IsAny<Guid>()), Times.Once);
            VideoPlatformServiceMock.Verify(v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), false, audioPlatformServiceResponse.IngestUrl, It.IsAny<IEnumerable<EndpointDto>>()), Times.Once);
            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<UpdateMeetingRoomCommand>()), Times.Once);
            QueryHandlerMock.Verify(q => q.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()), Times.Once);
        }

        [Test]
        public async Task Should_book_kinly_conference_with_ingesturl_when_audio_recording_is_required()
        {
            var audioPlatformServiceResponse = new AudioPlatformServiceResponse(true) {IngestUrl = "http://myIngestUrl.com"};
            AudioPlatformServiceMock.Setup(x => x.CreateAudioApplicationWithStreamAsync(_request.HearingRefId)).ReturnsAsync(audioPlatformServiceResponse);
            VideoPlatformServiceMock.Setup(v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), true, audioPlatformServiceResponse.IngestUrl, It.IsAny<IEnumerable<EndpointDto>>())).ReturnsAsync(MeetingRoom);

            _request.AudioRecordingRequired = true;
            await Controller.BookNewConferenceAsync(_request);

            AudioPlatformServiceMock.Verify(x => x.CreateAudioApplicationWithStreamAsync(It.IsAny<Guid>()), Times.Once);
            VideoPlatformServiceMock.Verify(v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), true, audioPlatformServiceResponse.IngestUrl, It.IsAny<IEnumerable<EndpointDto>>()), Times.Once);
            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<UpdateMeetingRoomCommand>()), Times.Once);
            QueryHandlerMock.Verify(q => q.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()), Times.Once);
        }
        
        [Test]
        public async Task Should_book_kinly_conference_with_default_ingesturl_when_create_audio_recording_request_fails()
        {
            var audioPlatformServiceResponse = new AudioPlatformServiceResponse(false) { IngestUrl = " " };
            AudioPlatformServiceMock.Setup(x => x.CreateAudioApplicationWithStreamAsync(_request.HearingRefId)).ReturnsAsync(audioPlatformServiceResponse);
            VideoPlatformServiceMock.Setup(v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), true, audioPlatformServiceResponse.IngestUrl, It.IsAny<IEnumerable<EndpointDto>>())).ReturnsAsync(MeetingRoom);

            _request.AudioRecordingRequired = true;
            await Controller.BookNewConferenceAsync(_request);

            AudioPlatformServiceMock.Verify(x => x.CreateAudioApplicationWithStreamAsync(It.IsAny<Guid>()), Times.Once);
            VideoPlatformServiceMock.Verify(v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), true, audioPlatformServiceResponse.IngestUrl, It.IsAny<IEnumerable<EndpointDto>>()), Times.Once);
            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<UpdateMeetingRoomCommand>()), Times.Once);
            QueryHandlerMock.Verify(q => q.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()), Times.Once);
        }
    }
}
