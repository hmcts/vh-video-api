using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using VideoApi.Contract.Requests;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Controllers.Consultation
{
    public class StartConsultationRequestTests : ConsultationControllerTestBase
    {

        [Test]
        public async Task Should_Return_Accepted()
        {
            var request = RequestBuilder();
            
            var result = await Controller.StartConsultationRequestAsync(request);
            
            var actionResult = result.As<AcceptedResult>();
            actionResult.Should().NotBeNull();
        }
        
        [Test]
        public async Task Should_Return_BadRequest_When_VirtualCourtRoomType_Is_Invalid()
        {
            var request = RequestBuilder();
            request.RoomType = (VirtualCourtRoomType)50;

            var result = await Controller.StartConsultationRequestAsync(request);
            
            var actionResult = result.As<BadRequestResult>();
            actionResult.Should().NotBeNull();
        }
        
        [Test]
        public async Task Should_Return_BadRequest_When_Conference_Does_Not_Exist()
        {
            var request = RequestBuilder();
            request.ConferenceId = Guid.NewGuid();

            var result = await Controller.StartConsultationRequestAsync(request);
            
            var actionResult = result.As<BadRequestResult>();
            actionResult.Should().NotBeNull();
        }

        [Test]
        public async Task Should_Return_Unauthorized_When_Requesting_Participant_Is_Not_A_Judge()
        {
            var request = RequestBuilder();
            var representative = TestConference.GetParticipants().First(x =>
                x.UserRole.Equals(UserRole.Representative));
            request.RequestedBy = representative.Id;
            
            var result = await Controller.StartConsultationRequestAsync(request);
            
            var actionResult = result.As<UnauthorizedResult>();
            actionResult.Should().NotBeNull();
        }
        
        private StartConsultationRequest RequestBuilder()
        {
            if (TestConference.Participants == null)
            {
                Assert.Fail("No participants found in conference");
            }
            
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
