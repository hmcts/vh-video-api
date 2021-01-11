using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Services.Kinly;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Consultation
{
    public class StartConsultationRequestTests : ConsultationControllerTestBase
    {

        private List<Room> _testRooms;
        
        [Test]
        public async Task Should_Return_Accepted()
        {
            var request = RequestBuilder();
            
            var result = await Controller.StartConsultationRequestAsync(request);
            
            var actionResult = result.As<AcceptedResult>();
            actionResult.Should().NotBeNull();
        }
        
        [Test]
        public async Task Should_Return_NotFound_When_Conference_Does_Not_Exist()
        {
            var request = RequestBuilder();
            request.ConferenceId = Guid.NewGuid();

            QueryHandlerMock
                .Setup(x => x.Handle<GetAvailableRoomByRoomTypeQuery, List<Room>>(
                    It.Is<GetAvailableRoomByRoomTypeQuery>(q => q.ConferenceId == request.ConferenceId)))
                .ThrowsAsync(new ConferenceNotFoundException(request.ConferenceId));
            
            var result = await Controller.StartConsultationRequestAsync(request);
            
            var actionResult = result.As<NotFoundObjectResult>();
            actionResult.Should().NotBeNull();
        }
        
        [Test]
        public async Task Should_Return_BadRequest_When_Participant_Cannot_Be_Found()
        {
            var request = RequestBuilder();
            request.RequestedBy = Guid.NewGuid();

            QueryHandlerMock
                .Setup(x => x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(
                    It.Is<GetConferenceByIdQuery>(q => q.ConferenceId == request.ConferenceId)))
                .ThrowsAsync(new ParticipantNotFoundException(request.RequestedBy));

            var result = await Controller.StartConsultationRequestAsync(request);
            
            var actionResult = result.As<NotFoundObjectResult>();
            actionResult.Should().NotBeNull();
        }
        
        private StartConsultationRequest RequestBuilder()
        {
            if (TestConference.Participants == null)
            {
                Assert.Fail("No participants found in conference");
            }
            
            _testRooms = new List<Room>
            {
                new Room(TestConference.Id, "JohRoom1", VirtualCourtRoomType.JudgeJOH)
            };
            
            QueryHandlerMock
                .Setup(x => x.Handle<GetAvailableRoomByRoomTypeQuery, List<Room>>(
                    It.Is<GetAvailableRoomByRoomTypeQuery>(q => q.ConferenceId == TestConference.Id)))
                .ReturnsAsync(_testRooms);
            
            return new StartConsultationRequest
            {
                ConferenceId = TestConference.Id,
                RequestedBy = TestConference.GetParticipants().First(x =>
                    x.UserRole.Equals(UserRole.Judge)).Id,
                RoomType = VirtualCourtRoomType.JudgeJOH
            };
        }
    }
}
