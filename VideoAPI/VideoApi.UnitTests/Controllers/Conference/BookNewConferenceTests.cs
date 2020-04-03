using System;
using Moq;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Api;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
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
               .Build();
        }

        [Test]
        public async Task Should_book_kinly_conference_room_for_given_conference_id()
        {
            VideoPlatformServiceMock.Setup(v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<string>())).ReturnsAsync((MeetingRoom)null);
            
            await Controller.BookNewConferenceAsync(_request);

            VideoPlatformServiceMock.Verify(v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<UpdateMeetingRoomCommand>()), Times.Never);
            QueryHandlerMock.Verify(q => q.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()), Times.Once);
        }

        [Test]
        public async Task Should_verify_double_booking_for_given_conference_id()
        {
            VideoPlatformServiceMock.Setup(v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<string>())).Throws(new DoubleBookingException(Guid.NewGuid()));

            await Controller.BookNewConferenceAsync(_request);

            VideoPlatformServiceMock.Verify(v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
            VideoPlatformServiceMock.Verify(v => v.GetVirtualCourtRoomAsync(It.IsAny<Guid>()), Times.Once);
            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<UpdateMeetingRoomCommand>()), Times.Never);
            QueryHandlerMock.Verify(q => q.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()), Times.Once);
        }

        [Test]
        public async Task Should_book_kinly_conference_and_update_meeting_room_for_given_conference_id()
        {
            VideoPlatformServiceMock.Setup(v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<string>())).ReturnsAsync(MeetingRoom);

            await Controller.BookNewConferenceAsync(_request);

            VideoPlatformServiceMock.Verify(v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<UpdateMeetingRoomCommand>()), Times.Once);
            QueryHandlerMock.Verify(q => q.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()), Times.Once);
        }
    }
}
