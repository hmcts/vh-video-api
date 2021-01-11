using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Queries;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Consultation
{
    public class EndJudgeJohConsultationTests : ConsultationControllerTestBase
    {
        [Test]
        public async Task should_return_not_found_when_no_matching_conference_is_found()
        {
            var judgeJohEndConsultationRequest = new EndConsultationRequest() { ConferenceId = Guid.NewGuid(), RoomId = 10 };
            var result = await Controller.EndJudgeJohConsultationRequestAsync(judgeJohEndConsultationRequest);
            var typedResult = (NotFoundResult)result;
            typedResult.Should().NotBeNull();
        }

        [Test]
        public async Task should_return_not_found_when_no_matching_room_is_found()
        {
            var conferenceId = TestConference.Id;
            var judgeJohEndConsultationRequest = new EndConsultationRequest() { ConferenceId = conferenceId, RoomId = 10 };
            var result = await Controller.EndJudgeJohConsultationRequestAsync(judgeJohEndConsultationRequest);
            var typedResult = (NotFoundResult)result;
            typedResult.Should().NotBeNull();
        }

        [Test]
        public async Task should_return_judge_joh_to_waiting_room_for_valid_conference_and_room_type()
        {
            var conferenceId = TestConference.Id;
            var room = TestRoom;
            var judgeJohEndConsultationRequest = new EndConsultationRequest() { ConferenceId = conferenceId, RoomId = room.Id };
            await Controller.EndJudgeJohConsultationRequestAsync(judgeJohEndConsultationRequest);

            QueryHandlerMock.Verify(q => q.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()), Times.Once);
            QueryHandlerMock.Verify(q => q.Handle<GetRoomByIdQuery, VideoApi.Domain.Room>(It.IsAny<GetRoomByIdQuery>()), Times.Once);
            ConsultationServiceMock.Verify(v => v.EndJudgeJohConsultationAsync(conferenceId, room), Times.Once);
            VideoPlatformServiceMock.VerifyNoOtherCalls();
        }
    }
}
