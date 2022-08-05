using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Consultation
{
    public class GetActiveJohConsultationRoomTests : ConsultationControllerTestBase
    {
        [Test]
        public async Task should_return_live_joh_consultation_room()
        {
            var conferenceId = "d749ea20-8234-4693-ac20-529eb505c235";
            var parsedConferenceId = Guid.Parse(conferenceId);
            var room = new ConsultationRoom(TestConference.Id, "Label",
                VideoApi.Domain.Enums.VirtualCourtRoomType.JudgeJOH, false);
            QueryHandlerMock
                .Setup(x => x.Handle<GetActiveJudgeJohConsultationRoomByConferenceIdQuery, List<Room>>(
                    It.Is<GetActiveJudgeJohConsultationRoomByConferenceIdQuery>(x =>
                        x.ConferenceId == parsedConferenceId))).Returns(Task.FromResult(new List<Room> {room}));

            var result = await Controller.GetActiveJohConsultationRoom(conferenceId) as OkObjectResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            var response = result.Value as List<RoomResponse>;
            response.Count.Should().Be(1);
            response.FirstOrDefault().Id.Should().Be(room.Id);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("asdasdasd")]
        public async Task should_return_bad_request_when_conferenceId_is_null(string conferenceId)
        {
            var result = await Controller.GetActiveJohConsultationRoom(conferenceId) as BadRequestResult;
        
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Test]
        public async Task should_return_not_found_when_no_rooms_returned()
        {
            var conferenceId = "d749ea20-8234-4693-ac20-529eb505c235";
            QueryHandlerMock
                .Setup(x => x.Handle<GetActiveJudgeJohConsultationRoomByConferenceIdQuery, List<Room>>(
                    It.IsAny<GetActiveJudgeJohConsultationRoomByConferenceIdQuery>())).Returns(Task.FromResult(new List<Room>()));

            var result = await Controller.GetActiveJohConsultationRoom(conferenceId) as NotFoundResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }
    }
}
