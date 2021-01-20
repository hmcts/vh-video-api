using System;
using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands;
using VideoApi.Domain.Enums;
using VideoApi.Domain.Validations;
using Testing.Common.Assertions;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Consultation
{
    public class HandleConsultationRequestTests : ConsultationControllerTestBase
    {
        [Test]
        public async Task Should_raise_notification_to_requester_and_admin_when_consultation_is_accepted()
        {
            var conferenceId = TestConference.Id;
            var requestedBy = TestConference.GetParticipants()[2];
            var requestedFor = TestConference.GetParticipants()[3];
            
            var answer = ConsultationAnswer.Accepted;

            var request = new ConsultationRequest
            {
                ConferenceId = conferenceId,
                RequestedBy = requestedBy.Id,
                RequestedFor = requestedFor.Id,
                Answer = answer,
                RoomName = "Room1"
            };

            await Controller.HandleConsultationRequestAsync(request);

            CommandHandlerMock.Verify(x => x.Handle(It.Is<SaveEventCommand>(s => s.Reason == $"Consultation with {requestedFor.DisplayName}")), Times.Once);
            ConsultationService.Verify(x =>
                x.JoinConsultationRoomAsync(TestConference.Id, requestedFor.Id, "Room1"), Times.Once);
            VideoPlatformServiceMock.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Should_return_notfound_when_no_requested_by_participant_is_found()
        {
            var conferenceId = TestConference.Id;
            var requestedBy = TestConference.GetParticipants()[2];

            var answer = ConsultationAnswer.Accepted;

            var request = new ConsultationRequest
            {
                ConferenceId = conferenceId,
                RequestedBy = requestedBy.Id,
                RequestedFor = Guid.NewGuid(),
                Answer = answer
            };

            var result = await Controller.HandleConsultationRequestAsync(request);
            var typedResult = (NotFoundResult)result;
            typedResult.Should().NotBeNull();
        }

        [Test]
        public async Task Should_return_notfound_when_no_requested_for_participant_is_found()
        {
            var conferenceId = TestConference.Id;
            var requestedFor = TestConference.GetParticipants()[3];

            var answer = ConsultationAnswer.Accepted;

            var request = new ConsultationRequest
            {
                ConferenceId = conferenceId,
                RequestedBy = Guid.NewGuid(),
                RequestedFor = requestedFor.Id,
                Answer = answer
            };

            var result = await Controller.HandleConsultationRequestAsync(request);
            var typedResult = (NotFoundResult)result;
            typedResult.Should().NotBeNull();
        }
    }
}
