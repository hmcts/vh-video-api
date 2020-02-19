using System;
using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Testing.Common.Assertions;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers
{
    public class CloseConferenceTests : ConferenceControllerTestBase
    {
        [Test]
        public async Task Should_close_conference_for_given_valid_conference_id()
        {
            VideoPlatformServiceMock.Setup(v => v.GetVirtualCourtRoomAsync(It.IsAny<Guid>())).ReturnsAsync((MeetingRoom)null);

            await Controller.CloseConference(Guid.NewGuid());

            QueryHandlerMock.Verify(q => q.Handle<GetConferenceByIdQuery, Conference>(It.IsAny<GetConferenceByIdQuery>()), Times.Once);
            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<CloseConferenceCommand>()), Times.Once);
            VideoPlatformServiceMock.Verify(v => v.DeleteVirtualCourtRoomAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Test]
        public async Task Should_close_conference_and_remove_court_room_for_given_valid_conference_id()
        {
            VideoPlatformServiceMock.Setup(v => v.GetVirtualCourtRoomAsync(It.IsAny<Guid>())).ReturnsAsync(MeetingRoom);

            await Controller.CloseConference(Guid.NewGuid());

            QueryHandlerMock.Verify(q => q.Handle<GetConferenceByIdQuery, Conference>(It.IsAny<GetConferenceByIdQuery>()), Times.Once);
            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<CloseConferenceCommand>()), Times.Once);
            VideoPlatformServiceMock.Verify(v => v.DeleteVirtualCourtRoomAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Test]
        public async Task Should_return_notfound_with_no_matching_conference()
        {
            QueryHandlerMock
             .Setup(x => x.Handle<GetConferenceByIdQuery, Conference>(It.IsAny<GetConferenceByIdQuery>()))
             .ReturnsAsync((Conference)null);

            var result = await Controller.CloseConference(Guid.NewGuid());

            var typedResult = (BadRequestResult)result;
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task Should_return_bad_request_for_given_invalid_user_name()
        {
            var conferenceId = Guid.Empty;

            var result = await Controller.CloseConference(conferenceId);

            var typedResult = (ObjectResult)result;
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)typedResult.Value).ContainsKeyAndErrorMessage(nameof(conferenceId), $"Please provide a valid {nameof(conferenceId)}");
        }

    }
}
